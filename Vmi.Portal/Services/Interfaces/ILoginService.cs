using Vmi.Portal.Entities;
namespace Vmi.Portal.Services.Interfaces;

public interface ILoginService
{
    Task<Usuario> Logar(string email, string senha);
}
