using Microsoft.AspNetCore.Mvc;
using Vmi.Portal.Entities;
using Vmi.Portal.Requests;
using Vmi.Portal.Services;
using Vmi.Portal.Services.Interfaces;
using Vmi.Portal.Utils;
using BC = BCrypt.Net.BCrypt;

namespace Vmi.Portal.Controllers;

[ApiController]
[Route("[controller]")]
public class LoginController : ControllerBase
{
    private readonly ILoginService _loginService;
    private readonly IPasswordResetService _passwordResetService;
    private readonly IUsuarioService _usuarioService;

    public LoginController(
        ILoginService loginService,
        IPasswordResetService passwordResetService,
        IUsuarioService usuarioService)
    {
        _loginService = loginService;
        _passwordResetService = passwordResetService;
        _usuarioService = usuarioService;
    }

    [HttpPost]
    public async Task<IActionResult> Autenticar([FromBody] LoginRequest request)
    {
        try
        {
            var usuario = await _loginService.Logar(request.Email, request.Password);
            if (usuario is null)
            {
                return Unauthorized(new
                {
                    Success = false,
                    Message = "Credenciais inválidas. Verifique seu e-mail e senha."
                });
            }

            if ((bool)!usuario.StatusUsuario)
            {
                return Unauthorized(new
                {
                    Success = false,
                    Message = "Conta desativada. Entre em contato com o suporte para mais informações."
                });
            }

            return Ok(new
            {
                Success = true,
                Data = usuario,
                Message = "Login realizado com sucesso."
            });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                Success = false,
                Message = "Ocorreu um erro durante o login. Tente novamente mais tarde."
            });
        }
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