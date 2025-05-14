using Dapper;
using System.Threading.Tasks;
using Vmi.Portal.Common;
using Vmi.Portal.DbContext;
using Vmi.Portal.Entities;

namespace Vmi.Portal.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly VmiDbContext _vmiDbContext;

    public UsuarioRepository(VmiDbContext vmiDbContext)
    {
        _vmiDbContext = vmiDbContext;
    }

    public async Task<PagedResult<Usuario>> ObterTodosUsuarios(int pageNumber, int pageSize, string nome, string email, int? perfilId, DateTime? dataCriacao, bool? statusAcesso)
    {
        string sql = @"
                    SELECT 
                        u.Id,
                        u.Nome,
                        u.Email,
                        u.Senha,
                        u.Perfil_id,
                        u.DataInclusao,
                        u.DataUltimaAlteracao,
                        u.StatusUsuario,
                        p.Nome AS PerfilNome,
                        uc.Nome AS NomeRespInclusao,
                        u.NomeRespUltimaAlteracao,
                        u.JustificativaInativacao
                    FROM
                        Usuarios u
                    LEFT JOIN 
                        Perfis p ON u.Perfil_id = p.Id
                    LEFT JOIN
                        Usuarios uc ON u.IdRespInclusao = uc.Id
                    WHERE 1=1";

        var countSql = "SELECT COUNT(*) FROM Usuarios u WHERE 1=1";

        if (!string.IsNullOrEmpty(nome))
        {
            sql += " AND u.Nome LIKE @NOME";
            countSql += " AND u.Nome LIKE @NOME";
        }

        if (!string.IsNullOrEmpty(email))
        {
            sql += " AND u.Email LIKE @EMAIL";
            countSql += " AND u.Email LIKE @EMAIL";
        }

        if (perfilId.HasValue)
        {
            sql += " AND u.Perfil_id = @IDPERFIL";
            countSql += " AND u.Perfil_id = @IDPERFIL";
        }

        if (dataCriacao.HasValue)
        {
            sql += " AND CONVERT(DATE, u.DataInclusao) = CONVERT(DATE, @DATA_CRIACAO)";
            countSql += " AND CONVERT(DATE, u.DataInclusao) = CONVERT(DATE, @DATA_CRIACAO)";
        }

        if (statusAcesso.HasValue)
        {
            sql += " AND u.StatusUsuario = @STATUS_ACESSO";
            countSql += " AND u.StatusUsuario = @STATUS_ACESSO";
        }

        sql += @"
                ORDER BY 
                    u.Id 
                OFFSET 
                    @OFFSET
                ROWS FETCH NEXT 
                    @PAGE_SIZE ROWS ONLY";

        using (var connection = _vmiDbContext.Connection)
        {
            var totalCount = await connection.ExecuteScalarAsync<int>(countSql, new
            {
                NOME = string.IsNullOrEmpty(nome) ? null : $"%{nome}%",
                EMAIL = string.IsNullOrEmpty(email) ? null : $"%{email}%",
                IDPERFIL = perfilId,
                DATA_CRIACAO = dataCriacao,
                STATUS_ACESSO = statusAcesso
            });

            var usuarios = await connection.QueryAsync<Usuario>(sql, new
            {
                NOME = string.IsNullOrEmpty(nome) ? null : $"%{nome}%",
                EMAIL = string.IsNullOrEmpty(email) ? null : $"%{email}%",
                IDPERFIL = perfilId,
                DATA_CRIACAO = dataCriacao,
                STATUS_ACESSO = statusAcesso,
                OFFSET = (pageNumber - 1) * pageSize,
                PAGE_SIZE = pageSize
            });

            return new PagedResult<Usuario>
            {
                Items = usuarios,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

    }

    public async Task<Usuario> ObterUsuarioPorId(int id)
    {
        const string sql = "SELECT * FROM Usuarios WHERE Id = @Id";
        return _vmiDbContext.Connection.QueryFirstOrDefault<Usuario>(sql, new { Id = id });
    }

    public async Task<Usuario> ObterUsuarioPorEmail(string email)
    {
        const string sql = "SELECT * FROM Usuarios WHERE Email = @Email";
        return _vmiDbContext.Connection.QueryFirstOrDefault<Usuario>(sql, new { Email = email });
    }

    public async Task AdicionarUsuario(Usuario usuario)
    {
        string sql =
            $@"
                INSERT INTO
                    Usuarios (
                        {nameof(Usuario.Nome)},
                        {nameof(Usuario.Email)},
                        {nameof(Usuario.Senha)},
                        {nameof(Usuario.Perfil_id)},
                        {nameof(Usuario.IdRespInclusao)},
                        {nameof(Usuario.NomeRespInclusao)},
                        {nameof(Usuario.DataInclusao)},
                        {nameof(Usuario.StatusUsuario)},
                        {nameof(Usuario.IsPrimeiroAcesso)}
                    )
                VALUES
                    (
                        @Nome,
                        @Email,
                        @Senha,
                        @Perfil_id,
                        @IdRespInclusao,
                        @NomeRespInclusao,
                        @DataInclusao,
                        @StatusUsuario,
                        @IsPrimeiroAcesso
                    );

                SELECT SCOPE_IDENTITY();
            ";

        await _vmiDbContext.Connection.ExecuteScalarAsync<int>(sql, usuario);
    }

    public async Task AtualizarUsuario(Usuario usuario)
    {
        var parameters = new DynamicParameters();
        var sqlSet = new List<string>();

        if (usuario.Nome != null)
        {
            sqlSet.Add($"Nome = @{nameof(Usuario.Nome)}");
            parameters.Add(nameof(Usuario.Nome), usuario.Nome);
        }

        if (usuario.Email != null)
        {
            sqlSet.Add($"Email = @{nameof(Usuario.Email)}");
            parameters.Add(nameof(Usuario.Email), usuario.Email);
        }

        if (usuario.Senha != null)
        {
            sqlSet.Add($"Senha = @{nameof(Usuario.Senha)}");
            parameters.Add(nameof(Usuario.Senha), usuario.Senha);
        }

        if (usuario.Perfil_id != null)
        {
            sqlSet.Add($"Perfil_id = @{nameof(Usuario.Perfil_id)}");
            parameters.Add(nameof(Usuario.Perfil_id), usuario.Perfil_id);
        }

        if (usuario.StatusUsuario)
        {
            sqlSet.Add($"StatusUsuario = @{nameof(usuario.StatusUsuario)}");
            parameters.Add(nameof(Usuario.StatusUsuario), usuario.StatusUsuario);

            sqlSet.Add($"DataInativacao = NULL");
            sqlSet.Add($"IdRespInativacao = NULL");
            sqlSet.Add($"NomeRespInativacao = NULL");
            sqlSet.Add($"JustificativaInativacao = NULL");
        }
        else
        {
            sqlSet.Add($"StatusUsuario = @{nameof(Usuario.StatusUsuario)}");
            parameters.Add(nameof(Usuario.StatusUsuario), usuario.StatusUsuario);

            if (usuario.IdRespInativacao != null)
            {
                sqlSet.Add($"IdRespInativacao = @{nameof(Usuario.IdRespInativacao)}");
                parameters.Add(nameof(Usuario.IdRespInativacao), usuario.IdRespInativacao);

                sqlSet.Add($"NomeRespInativacao = @{nameof(Usuario.NomeRespInativacao)}");
                parameters.Add(nameof(Usuario.NomeRespInativacao), usuario.NomeRespInativacao);
            }

            sqlSet.Add($"DataInativacao = @{nameof(Perfil.DataInativacao)}");
            parameters.Add(nameof(Usuario.DataInativacao), DateTime.Now);

            if (usuario.JustificativaInativacao != null)
            {
                sqlSet.Add($"JustificativaInativacao = @{nameof(Usuario.JustificativaInativacao)}");
                parameters.Add(nameof(Usuario.JustificativaInativacao), usuario.JustificativaInativacao);
            }
        }

        if (usuario.DataUltimaAlteracao != null)
        {
            sqlSet.Add($"DataUltimaAlteracao = @{nameof(Usuario.DataUltimaAlteracao)}");
            parameters.Add(nameof(Usuario.DataUltimaAlteracao), usuario.DataUltimaAlteracao);
        }

        if (usuario.IsPrimeiroAcesso != null)
        {
            sqlSet.Add($"IsPrimeiroAcesso = @{nameof(Usuario.IsPrimeiroAcesso)}");
            parameters.Add(nameof(Usuario.IsPrimeiroAcesso), usuario.IsPrimeiroAcesso);
        }

        if (usuario.IdRespUltimaAlteracao != null)
        {
            sqlSet.Add($"IdRespUltimaAlteracao = @{nameof(Usuario.IdRespUltimaAlteracao)}");
            parameters.Add(nameof(Usuario.IdRespUltimaAlteracao), usuario.IdRespUltimaAlteracao);
        }

        if (usuario.NomeRespUltimaAlteracao != null)
        {
            sqlSet.Add($"NomeRespUltimaAlteracao = @{nameof(Usuario.NomeRespUltimaAlteracao)}");
            parameters.Add(nameof(Usuario.NomeRespUltimaAlteracao), usuario.NomeRespUltimaAlteracao);
        }

        parameters.Add(nameof(Usuario.Id), usuario.Id);

        if (sqlSet.Any())
        {
            string sql = $"UPDATE Usuarios SET {string.Join(", ", sqlSet)} WHERE Id = @{nameof(Usuario.Id)}";
            await _vmiDbContext.Connection.ExecuteAsync(sql, parameters);
        }
    }

    public async Task DeletarUsuario(int id)
    {
        string sql = "DELETE FROM Usuarios WHERE Id = @Id";
        await _vmiDbContext.Connection.ExecuteAsync(sql, new { Id = id });
    }

    public bool Save()
    {
        return true;
    }
}