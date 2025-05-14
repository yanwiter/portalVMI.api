using Vmi.Portal.Entities;
using Vmi.Portal.Repositories.Interfaces;
using Vmi.Portal.Services.Interfaces;
using BC = BCrypt.Net.BCrypt;

namespace Vmi.Portal.Services;

public class LoginService : ILoginService
{
    private readonly ILoginRepository _loginRepository;

    public LoginService(ILoginRepository loginRepository)
    {
        _loginRepository = loginRepository;
    }

    public async Task<Usuario> Logar(string email, string senha)
    {
        Usuario usuario =
            await _loginRepository.GetUsuarioPorEmail(email);

        if (usuario == null)
            throw new ArgumentException("Usuário inexistente. Faça um cadastro para esse usuário!");

        if (usuario != null && usuario?.Senha != null && !BC.Verify(senha, usuario?.Senha))
            throw new ArgumentException("Senha inválida. Caso tenha esquecido, clique em Esqueci minha senha!");

        if (usuario != null)
        {
            usuario.Senha = null;
        }
        return usuario;
    }
}