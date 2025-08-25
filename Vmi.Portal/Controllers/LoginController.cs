using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Vmi.Portal.Data.Repositories.Interfaces;
using Vmi.Portal.Entities;
using Vmi.Portal.Requests;
using Vmi.Portal.Services;
using Vmi.Portal.Services.Interfaces;
using Vmi.Portal.Utils;
using Vmi.Portal.Enums;
using BC = BCrypt.Net.BCrypt;

namespace Vmi.Portal.Controllers;

[ApiController]
[Route("[controller]")]
public class LoginController : ControllerBase
{
    private readonly ILoginService _loginService;
    private readonly IPasswordResetService _passwordResetService;
    private readonly IUsuarioService _usuarioService;
    private readonly IJwtService _jwtService;
    private readonly IActiveSessionService _activeSessionService;
    private readonly IAuthenticationCodeService _authenticationCodeService;
    private readonly IEmailService _emailService;
    private readonly IPerfilService _perfilService;

    public LoginController(
        ILoginService loginService,
        IPasswordResetService passwordResetService,
        IUsuarioService usuarioService,
        IJwtService jwtService,
        IActiveSessionService activeSessionService,
        IAuthenticationCodeService authenticationCodeService,
        IEmailService emailService,
        IPerfilService perfilService)
    {
        _loginService = loginService;
        _passwordResetService = passwordResetService;
        _usuarioService = usuarioService;
        _jwtService = jwtService;
        _activeSessionService = activeSessionService;
        _authenticationCodeService = authenticationCodeService;
        _emailService = emailService;
        _perfilService = perfilService;
    }

    [HttpPost]
    public async Task<IActionResult> Autenticar([FromBody] LoginRequest request)
    {
        try
        {
            var usuario = await _usuarioService.ObterUsuarioPorEmail(request.Email);

            //if (usuario == null || !BC.Verify(request.Password, usuario.Senha))
            //    return Unauthorized(new { Success = false, Message = "Credenciais inválidas." });

            if (usuario.StatusUsuario != StatusUsuarioEnum.Ativo)
                return Unauthorized(new { Success = false, Message = "Conta desativada." });

            if (usuario.StatusUsuario == StatusUsuarioEnum.Suspenso)
            {
                if (usuario.TipoSuspensao.HasValue)
                {
                    if (usuario.TipoSuspensao == TipoSuspensaoEnum.Temporaria)
                    {
                        if (usuario.DataFimSuspensao.HasValue && usuario.DataFimSuspensao > DateTime.Now)
                        {
                            return Unauthorized(new { 
                                Success = false, 
                                Message = $"Conta temporariamente suspensa até {usuario.DataFimSuspensao.Value.ToString("dd/MM/yyyy")}. Motivo: {usuario.MotivoSuspensao}" 
                            });
                        }
                    }
                    else if (usuario.TipoSuspensao == TipoSuspensaoEnum.Permanente)
                    {
                        return Unauthorized(new { 
                            Success = false, 
                            Message = $"Conta permanentemente suspensa. Motivo: {usuario.MotivoSuspensao}" 
                        });
                    }
                }
                else
                {
                    return Unauthorized(new { 
                        Success = false, 
                        Message = "Conta suspensa. Entre em contato com o administrador." 
                    });
                }
            }

            if (usuario.IdPerfil.HasValue)
            {
                var perfil = await _perfilService.ObterPerfilPorId(usuario.IdPerfil.Value);
                if (perfil != null)
                {
                    if (perfil.StatusPerfil == StatusPerfilEnum.Inativo)
                    {
                        return Unauthorized(new { 
                            Success = false, 
                            Message = "Perfil de usuário inativo. Entre em contato com o administrador." 
                        });
                    }
                    
                    if (perfil.StatusPerfil == StatusPerfilEnum.Suspenso)
                    {
                        if (perfil.TipoSuspensao.HasValue)
                        {
                            if (perfil.TipoSuspensao == TipoSuspensaoEnum.Temporaria)
                            {
                                if (perfil.DataFimSuspensao.HasValue && perfil.DataFimSuspensao > DateTime.Now)
                                {
                                    return Unauthorized(new { 
                                        Success = false, 
                                        Message = $"Perfil temporariamente suspenso até {perfil.DataFimSuspensao.Value.ToString("dd/MM/yyyy")}. Motivo: {perfil.MotivoSuspensao}" 
                                    });
                                }
                            }
                            else if (perfil.TipoSuspensao == TipoSuspensaoEnum.Permanente)
                            {
                                return Unauthorized(new { 
                                    Success = false, 
                                    Message = $"Perfil permanentemente suspenso. Motivo: {perfil.MotivoSuspensao}" 
                                });
                            }
                        }
                        else
                        {
                            return Unauthorized(new { 
                                Success = false, 
                                Message = "Perfil de usuário suspenso. Entre em contato com o administrador." 
                            });
                        }
                    }
                }
            }

            if (!_loginService.VerificarHorarioAcessoPermitido(usuario.HorariosAcesso))
                return Unauthorized(new { Success = false, Message = "Autenticação fora do horário." });

            var authCode = new Random().Next(100000, 999999).ToString();
            var expiration = DateTime.UtcNow.AddMinutes(10);

            await _authenticationCodeService.SaveAuthCode(usuario.Id, authCode, expiration);
            await _authenticationCodeService.SendAuthenticationCode(usuario.Email, usuario.Nome, authCode);

            usuario.DataUltimoLogin = Util.PegaHoraBrasilia();
            await _usuarioService.AtualizarUsuario(usuario);

            return Ok(new
            {
                Success = true,
                Data = new
                {
                    UserId = usuario.Id,
                    Message = "Código de autenticação enviado para seu e-mail"
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Success = false, Message = "Erro durante o login" });
        }
    }

    [HttpPost("validar-codigo")]
    public async Task<IActionResult> ValidarCodigo([FromBody] ValidateCodeRequest request)
    {
        try
        {
            var isValid = await _authenticationCodeService.ValidateCode(request.UserId, request.Code);

            if (!isValid)
                return BadRequest(new { Success = false, Message = "Código inválido ou expirado" });

            var usuario = await _usuarioService.ObterUsuarioPorId(request.UserId);
            var tokenResponse = await _jwtService.GenerateTokens(
                usuario,
                Request.HttpContext.Connection.RemoteIpAddress?.ToString(),
                Request.Headers.UserAgent.ToString());

            return Ok(new
            {
                Success = true,
                Data = new
                {
                    User = new
                    {
                        usuario.Id,
                        usuario.Nome,
                        usuario.Email,
                        usuario.PerfilNome,
                        usuario.StatusUsuario,
                        usuario.IsPrimeiroAcesso,
                        usuario.IdPerfil,
                        usuario.FotoPerfil
                    },
                    Token = tokenResponse.AccessToken,
                    RefreshToken = tokenResponse.RefreshToken,
                    ExpiresAt = tokenResponse.ExpiresAt
                },
                Message = "Autenticação realizada com sucesso"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Success = false, Message = "Erro ao validar código" });
        }
    }

    [HttpPost("reenviar-codigo")]
    public async Task<IActionResult> ReenviarCodigo([FromBody] ResendCodeRequest request)
    {
        try
        {
            var usuario = await _usuarioService.ObterUsuarioPorId(request.UserId);
            if (usuario == null)
                return NotFound(new { Success = false, Message = "Usuário não encontrado" });

            await _authenticationCodeService.ResendCode(usuario.Id);

            return Ok(new
            {
                Success = true,
                Message = "Novo código enviado para seu e-mail"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Success = false, Message = "Erro ao reenviar código" });
        }
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            var principal = _jwtService.GetPrincipalFromExpiredToken(request.AccessToken);
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!await _activeSessionService.IsActiveSession(userId, request.RefreshToken))
                return Unauthorized(new { Success = false, Message = "Refresh token inválido" });

            var usuario = await _usuarioService.ObterUsuarioPorEmail(principal.FindFirstValue(ClaimTypes.Email));
            var newTokenResponse = await _jwtService.GenerateTokens(
                usuario,
                Request.HttpContext.Connection.RemoteIpAddress?.ToString(),
                Request.Headers.UserAgent.ToString());

            return Ok(new
            {
                Success = true,
                newTokenResponse.AccessToken,
                newTokenResponse.RefreshToken,
                newTokenResponse.ExpiresAt
            });
        }
        catch (SecurityTokenException)
        {
            return Unauthorized(new { Success = false, Message = "Token inválido" });
        }
    }

    [Authorize]
    [HttpPost("revoke")]
    public async Task<IActionResult> Revoke()
    {
        await _activeSessionService.RevokeAllSessions(User.FindFirstValue(ClaimTypes.NameIdentifier));
        return Ok(new { Success = true, Message = "Todas as sessões revogadas" });
    }

    [HttpGet("sessions")]
    public async Task<IActionResult> GetActiveSessions()
    {
        var sessions = await _activeSessionService.GetUserSessions(User.FindFirstValue(ClaimTypes.NameIdentifier));
        return Ok(new { Success = true, Data = sessions });
    }

    [Authorize]
    [HttpPost("revoke/{refreshToken}")]
    public async Task<IActionResult> RevokeSession(string refreshToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        await _activeSessionService.RemoveSession(userId, refreshToken);
        return Ok(new { Success = true, Message = "Sessão revogada" });
    }

    [HttpPost("Esqueci-senha")]
    public async Task<IActionResult> EsquecimentoSenha([FromBody] ResetPasswordRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return BadRequest(new { Success = false, Message = "E-mail é obrigatório." });
            }

            var result = await _passwordResetService.GerarTokenRedefinicaoSenha(request.Email);

            if (!result.Success)
            {
                return BadRequest(new { Success = false, Message = result.Message });
            }

            return Ok(new
            {
                Success = true,
                Message = "Se o e-mail estiver cadastrado, você receberá um link para redefinir sua senha."
            });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                Success = false,
                Message = "Ocorreu um erro ao processar sua solicitação."
            });
        }
    }

    [HttpGet("Validar-token-reedefinicao")]
    public async Task<IActionResult> ValidarTokenRedefinicaoSenha([FromQuery] string token)
    {
        try
        {
            var isValid = await _passwordResetService.ValidarTokenRedefinicaoSenha(token);

            if (!isValid)
            {
                return BadRequest(new 
                { 
                    Success = false, 
                    Message = "Token inválido ou expirado." 
                });
            }

            return Ok(new 
            { 
                Success = true, 
                Message = "Token válido." 
            });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                Success = false,
                Message = "Ocorreu um erro ao validar o token."
            });
        }
    }

    [HttpPost("Resetar-senha")]
    public async Task<IActionResult> RedefinirSenha([FromBody] UpdatePasswordRequest request)
    {
        try
        {
            if (request.NewPassword != request.ConfirmPassword)
            {
                return BadRequest(new { Success = false, Message = "As senhas não coincidem." });
            }

            var result = await _passwordResetService.RedefinirSenha(
                request.Token,
                request.NewPassword);

            if (!result.Success)
            {
                return BadRequest(new 
                { 
                    Success = false, 
                    Message = result.Message 
                });
            }

            return Ok(new
            {
                Success = true,
                Message = "Senha redefinida com sucesso."
            });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                Success = false,
                Message = "Ocorreu um erro ao redefinir a senha."
            });
        }
    }

    [HttpPost("Resetar-senha-primeiro-acesso")]
    public async Task<IActionResult> RedefinirSenhaPrimeiroAcesso([FromBody] UpdatePasswordRequest request)
    {
        try
        {
            if (request.NewPassword != request.ConfirmPassword)
            {
                return BadRequest(new { Success = false, Message = "As senhas não coincidem." });
            }

            var usuario = await _usuarioService.ObterUsuarioPorEmail(request.email);

            usuario.DataUltimaAlteracao = Util.PegaHoraBrasilia();
            usuario.Senha = BC.HashPassword(request.NewPassword);
            usuario.IsPrimeiroAcesso = false;

            await _usuarioService.AtualizarUsuario(usuario);

            return Ok(new
            {
                Success = true,
                Message = "Senha redefinida com sucesso."
            });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                Success = false,
                Message = "Ocorreu um erro ao redefinir a senha."
            });
        }
    }
}