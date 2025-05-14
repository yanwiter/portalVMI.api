using Vmi.Portal.Entities;

namespace Vmi.Portal.Repositories.Interfaces;

public interface ILoginRepository
{
    Task<Usuario> Logar(string email, string senha);
    Task<Usuario> GetUsuarioPorEmail(string email);
}
