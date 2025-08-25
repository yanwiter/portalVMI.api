using Newtonsoft.Json;
using Vmi.Portal.Entities;
using Vmi.Portal.Repositories.Interfaces;
using Vmi.Portal.Services.Interfaces;
using Vmi.Portal.Enums;
using BC = BCrypt.Net.BCrypt;

namespace Vmi.Portal.Services;

public class LoginService : ILoginService
{
    private readonly ILoginRepository _loginRepository;
    private readonly IJwtService _jwtService;
    private readonly IActiveSessionService _activeSessionService;
    private readonly IPerfilService _perfilService;

    public LoginService(
        ILoginRepository loginRepository,
        IJwtService jwtService,
        IActiveSessionService activeSessionService,
        IPerfilService perfilService)
    {
        _loginRepository = loginRepository;
        _jwtService = jwtService;
        _activeSessionService = activeSessionService;
        _perfilService = perfilService;
    }

    public async Task<TokenResponse> Logar(string email, string senha, string ipAddress, string deviceInfo)
    {
        var usuario = await _loginRepository.GetUsuarioPorEmail(email);

        if (usuario == null || !BC.Verify(senha, usuario.Senha))
            throw new UnauthorizedAccessException("Credenciais inválidas");

        if (usuario.StatusUsuario != StatusUsuarioEnum.Ativo)
            throw new UnauthorizedAccessException("Conta desativada");

        if (usuario.StatusUsuario == StatusUsuarioEnum.Suspenso)
        {
            if (usuario.TipoSuspensao.HasValue)
            {
                var agora = DateTime.Now;
                
                if (usuario.TipoSuspensao == TipoSuspensaoEnum.Temporaria)
                {
                    if (usuario.DataFimSuspensao.HasValue && usuario.DataFimSuspensao > agora)
                    {
                        throw new UnauthorizedAccessException($"Conta temporariamente suspensa até {usuario.DataFimSuspensao.Value.ToString("dd/MM/yyyy")}. Motivo: {usuario.MotivoSuspensao}");
                    }
                }
                else if (usuario.TipoSuspensao == TipoSuspensaoEnum.Permanente)
                {
                    throw new UnauthorizedAccessException($"Conta permanentemente suspensa. Motivo: {usuario.MotivoSuspensao}");
                }
            }
            else
            {
                throw new UnauthorizedAccessException("Conta suspensa. Entre em contato com o administrador.");
            }
        }

        if (usuario.IdPerfil.HasValue)
        {
            var perfil = await _perfilService.ObterPerfilPorId(usuario.IdPerfil.Value);
            if (perfil != null)
            {
                if (perfil.StatusPerfil == StatusPerfilEnum.Inativo)
                {
                    throw new UnauthorizedAccessException("Perfil de usuário inativo. Entre em contato com o administrador.");
                }
                
                if (perfil.StatusPerfil == StatusPerfilEnum.Suspenso)
                {
                    if (perfil.TipoSuspensao.HasValue)
                    {
                        var agora = DateTime.Now;
                        
                        if (perfil.TipoSuspensao == TipoSuspensaoEnum.Temporaria)
                        {
                            if (perfil.DataFimSuspensao.HasValue && perfil.DataFimSuspensao > agora)
                            {
                                throw new UnauthorizedAccessException($"Perfil temporariamente suspenso até {perfil.DataFimSuspensao.Value.ToString("dd/MM/yyyy")}. Motivo: {perfil.MotivoSuspensao}");
                            }
                        }
                        else if (perfil.TipoSuspensao == TipoSuspensaoEnum.Permanente)
                        {
                            throw new UnauthorizedAccessException($"Perfil permanentemente suspenso. Motivo: {perfil.MotivoSuspensao}");
                        }
                    }
                    else
                    {
                        throw new UnauthorizedAccessException("Perfil de usuário suspenso. Entre em contato com o administrador.");
                    }
                }
            }
        }

        var tokenResponse = await _jwtService.GenerateTokens(usuario, ipAddress, deviceInfo);
        await _activeSessionService.CreateSession(usuario.Id, tokenResponse.RefreshToken, ipAddress, deviceInfo);

        return tokenResponse;
    }

    public bool VerificarHorarioAcessoPermitido(string horariosAcessoJson)
    {
        if (string.IsNullOrEmpty(horariosAcessoJson))
            return true;

        try
        {
            var horarios = JsonConvert.DeserializeObject<List<HorarioAcesso>>(horariosAcessoJson);
            if (horarios == null || !horarios.Any())
                return true;

            var agora = DateTime.Now;
            var diaSemana = (int)agora.DayOfWeek;
            var horaAtual = agora.TimeOfDay;

            var horarioPermitido = horarios.FirstOrDefault(h => h.DiaSemana == diaSemana);
            if (horarioPermitido == null)
                return false;

            if (TimeSpan.TryParse(horarioPermitido.Inicio, out var inicio) && 
                TimeSpan.TryParse(horarioPermitido.Fim, out var fim))
            {
                return horaAtual >= inicio && horaAtual <= fim;
            }

            return true;
        }
        catch
        {
            return true;
        }
    }
}