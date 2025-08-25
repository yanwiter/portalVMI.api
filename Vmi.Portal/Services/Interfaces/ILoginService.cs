using Vmi.Portal.Entities;
namespace Vmi.Portal.Services.Interfaces;

public interface ILoginService
{
    Task<TokenResponse> Logar(string email, string senha, string ipAddress, string deviceInfo);
    bool VerificarHorarioAcessoPermitido(string horariosAcessoJson);
}
