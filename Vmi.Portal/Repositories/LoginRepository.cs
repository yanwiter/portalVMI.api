using Dapper;
using System.Data;
using Vmi.Portal.DbContext;
using Vmi.Portal.Entities;
using Vmi.Portal.Repositories.Interfaces;

namespace Vmi.Portal.Repository;

public class LoginRepository : ILoginRepository
{
    private readonly VmiDbContext _vmiDbContext;

    public LoginRepository(VmiDbContext vmiDbContext)
    {
        _vmiDbContext = vmiDbContext;
    }

    public async Task<Usuario> GetUsuarioPorEmail(string email)
    {
        return await _vmiDbContext.Connection.QueryFirstOrDefaultAsync<Usuario>(
            sql: $@"
                    SELECT
                        u.Id,
                        u.Nome,
                        u.Email,
                        u.Senha,
                        u.Perfil_id,
                        u.NomeRespInclusao,
                        u.DataInclusao,
                        u.StatusUsuario,
                        p.Nome AS PerfilNome,
                        u.IsPrimeiroAcesso
                    FROM
                        Usuarios u
                    LEFT JOIN 
                        Perfis p ON u.Perfil_id = p.Id
                    WHERE
                        Email = @EMAIL
            ",
            param: new
            {
                EMAIL = email
            },
            commandType: CommandType.Text
        );
    }

    public async Task<Usuario> Logar(string email, string senha)
    {
        return await _vmiDbContext.Connection.QueryFirstOrDefaultAsync<Usuario>(
            sql: $@"
                    SELECT
                        u.Id,
                        u.Nome,
                        u.Email,
                        u.Senha,
                        u.Perfil_id,
                        u.DataInclusao,
                        u.StatusUsuario,
                        p.Nome AS PerfilNome,
                        u.IsPrimeiroAcesso
                    FROM
                        Usuarios u
                    LEFT JOIN 
                        Perfis p ON u.Perfil_id = p.Id
                    WHERE
                        Email = @EMAIL AND 
                        Senha = @SENHA
            ",
            param: new
            {
                EMAIL = email,
                SENHA = senha
            },
            commandType: CommandType.Text
        );
    }

    public async Task Deletar(string email)
    {
        await _vmiDbContext.Connection.ExecuteAsync(
            sql: $@"
                    DELETE FROM
                        Usuarios
                    WHERE
                        Email = @EMAIL
            ",
            param: new
            {
                EMAIL = email
            },
            commandType: CommandType.Text
        );
    }

    public string GetUserByUsername(string username)
    {
        return "Usuário encontrado: " + username;
    }
}
