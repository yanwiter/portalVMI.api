using Dapper;
using DocumentFormat.OpenXml.Drawing.Charts;
using System.Threading.Tasks;
using Vmi.Portal.Common;
using Vmi.Portal.DbContext;
using Vmi.Portal.Entities;
using Vmi.Portal.Enums;

namespace Vmi.Portal.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly VmiDbContext _vmiDbContext;

    public UsuarioRepository(VmiDbContext vmiDbContext)
    {
        _vmiDbContext = vmiDbContext;
    }

    public async Task<PagedResult<Usuario>> ObterTodosUsuarios(int pageNumber, int pageSize, string nome, string email, Guid? idPerfil, DateTime? dataCriacao, bool? statusAcesso)
    {
        string sql = @"
                SELECT 
                    u.Id,
                    u.Nome,
                    u.Email,
                    u.Senha,
                    u.IdPerfil,
                    u.DataInclusao,
                    u.DataUltimaAlteracao,
                    u.DataInativacao,
                    u.StatusUsuario,
                    u.DataUltimoLogin,
                    p.Nome AS PerfilNome,
                    uc.Nome AS NomeRespInclusao,
                    u.NomeRespUltimaAlteracao,
                    u.JustificativaInativacao,
                    u.Telefone,
                    u.CpfCnpj,
                    u.UsuarioLogin,
                    u.DataExpiracao,
                    u.Observacoes,
                    u.TipoAcesso,
                    u.TipoPessoa,
                    u.HorariosAcesso,
                    u.TipoSuspensao,
                    u.DataInicioSuspensao,
                    u.DataFimSuspensao,
                    u.MotivoSuspensao,
                    u.IdRespSuspensao,
                    u.NomeRespSuspensao,
                    u.DataSuspensao,
                    u.FotoPerfil
                FROM
                    Usuarios u
                LEFT JOIN 
                    Perfis p ON u.IdPerfil = p.Id
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

        if (idPerfil.HasValue)
        {
            sql += " AND u.IdPerfil = @IDPERFIL";
            countSql += " AND u.IdPerfil = @IDPERFIL";
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
                IDPERFIL = idPerfil,
                DATA_CRIACAO = dataCriacao,
                STATUS_ACESSO = statusAcesso
            });

            var usuarios = await connection.QueryAsync<Usuario>(sql, new
            {
                NOME = string.IsNullOrEmpty(nome) ? null : $"%{nome}%",
                EMAIL = string.IsNullOrEmpty(email) ? null : $"%{email}%",
                IDPERFIL = idPerfil,
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

    public async Task<Usuario> ObterUsuarioPorId(Guid id)
    {
        const string sql = @"
        SELECT
            u.*, p.Nome AS PerfilNome
        FROM
            Usuarios u
        LEFT JOIN
            Perfis p ON u.IdPerfil = p.Id
        WHERE u.Id = @Id";

        return await _vmiDbContext.Connection.QueryFirstOrDefaultAsync<Usuario>(sql, new { Id = id });
    }

    public async Task<Usuario> ObterUsuarioPorEmail(string email)
    {
        const string sql = @"
        SELECT 
            u.*, p.Nome AS PerfilNome 
        FROM 
            Usuarios u
        LEFT JOIN 
            Perfis p ON u.IdPerfil = p.Id
        WHERE 
            u.Email = @Email";

        return await _vmiDbContext.Connection.QueryFirstOrDefaultAsync<Usuario>(sql, new { Email = email });
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
                        {nameof(Usuario.IdPerfil)},
                        {nameof(Usuario.IdRespInclusao)},
                        {nameof(Usuario.NomeRespInclusao)},
                        {nameof(Usuario.DataInclusao)},
                        {nameof(Usuario.StatusUsuario)},
                        {nameof(Usuario.IsPrimeiroAcesso)},
                        {nameof(Usuario.Telefone)},
                        {nameof(Usuario.CpfCnpj)},
                        {nameof(Usuario.UsuarioLogin)},
                        {nameof(Usuario.DataExpiracao)},
                        {nameof(Usuario.Observacoes)},
                        {nameof(Usuario.TipoAcesso)},
                        {nameof(Usuario.TipoPessoa)},
                        {nameof(Usuario.HorariosAcesso)},
                        {nameof(Usuario.TipoSuspensao)},
                        {nameof(Usuario.DataInicioSuspensao)},
                        {nameof(Usuario.DataFimSuspensao)},
                        {nameof(Usuario.MotivoSuspensao)},
                        {nameof(Usuario.IdRespSuspensao)},
                        {nameof(Usuario.NomeRespSuspensao)},
                        {nameof(Usuario.DataSuspensao)},
                        {nameof(Usuario.FotoPerfil)}
                    )
                VALUES
                    (
                        @Nome,
                        @Email,
                        @Senha,
                        @IdPerfil,
                        @IdRespInclusao,
                        @NomeRespInclusao,
                        @DataInclusao,
                        @StatusUsuario,
                        @IsPrimeiroAcesso,
                        @Telefone,
                        @CpfCnpj,
                        @UsuarioLogin,
                        @DataExpiracao,
                        @Observacoes,
                        @TipoAcesso,
                        @TipoPessoa,
                        @HorariosAcesso,
                        @TipoSuspensao,
                        @DataInicioSuspensao,
                        @DataFimSuspensao,
                        @MotivoSuspensao,
                        @IdRespSuspensao,
                        @NomeRespSuspensao,
                        @DataSuspensao,
                        @FotoPerfil
                    );

                SELECT SCOPE_IDENTITY();
            ";

        await _vmiDbContext.Connection.ExecuteScalarAsync<Guid>(sql, usuario);
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

        if (usuario.IdPerfil != null)
        {
            sqlSet.Add($"IdPerfil = @{nameof(Usuario.IdPerfil)}");
            parameters.Add(nameof(Usuario.IdPerfil), usuario.IdPerfil);
        }

        if (usuario.Telefone != null)
        {
            sqlSet.Add($"Telefone = @{nameof(Usuario.Telefone)}");
            parameters.Add(nameof(Usuario.Telefone), usuario.Telefone);
        }

        if (usuario.CpfCnpj != null)
        {
            sqlSet.Add($"CpfCnpj = @{nameof(Usuario.CpfCnpj)}");
            parameters.Add(nameof(Usuario.CpfCnpj), usuario.CpfCnpj);
        }

        if (usuario.UsuarioLogin != null)
        {
            sqlSet.Add($"UsuarioLogin = @{nameof(Usuario.UsuarioLogin)}");
            parameters.Add(nameof(Usuario.UsuarioLogin), usuario.UsuarioLogin);
        }

        if (usuario.DataExpiracao != null)
        {
            sqlSet.Add($"DataExpiracao = @{nameof(Usuario.DataExpiracao)}");
            parameters.Add(nameof(Usuario.DataExpiracao), usuario.DataExpiracao);
        }

        if (usuario.Observacoes != null)
        {
            sqlSet.Add($"Observacoes = @{nameof(Usuario.Observacoes)}");
            parameters.Add(nameof(Usuario.Observacoes), usuario.Observacoes);
        }

        if (usuario.TipoAcesso != null)
        {
            sqlSet.Add($"TipoAcesso = @{nameof(Usuario.TipoAcesso)}");
            parameters.Add(nameof(Usuario.TipoAcesso), usuario.TipoAcesso);
        }

        if (usuario.TipoPessoa != null)
        {
            sqlSet.Add($"TipoPessoa = @{nameof(Usuario.TipoPessoa)}");
            parameters.Add(nameof(Usuario.TipoPessoa), usuario.TipoPessoa);
        }

        if (usuario.HorariosAcesso != null)
        {
            sqlSet.Add($"HorariosAcesso = @{nameof(Usuario.HorariosAcesso)}");
            parameters.Add(nameof(Usuario.HorariosAcesso), usuario.HorariosAcesso);
        }

        if (usuario.FotoPerfil != null)
        {
            sqlSet.Add($"FotoPerfil = @{nameof(Usuario.FotoPerfil)}");
            parameters.Add(nameof(Usuario.FotoPerfil), usuario.FotoPerfil);
        }

        if (usuario.TipoSuspensao != null)
        {
            sqlSet.Add($"TipoSuspensao = @{nameof(Usuario.TipoSuspensao)}");
            parameters.Add(nameof(Usuario.TipoSuspensao), usuario.TipoSuspensao);
        }

        if (usuario.DataInicioSuspensao != null)
        {
            sqlSet.Add($"DataInicioSuspensao = @{nameof(Usuario.DataInicioSuspensao)}");
            parameters.Add(nameof(Usuario.DataInicioSuspensao), usuario.DataInicioSuspensao);
        }

        if (usuario.DataFimSuspensao != null)
        {
            sqlSet.Add($"DataFimSuspensao = @{nameof(Usuario.DataFimSuspensao)}");
            parameters.Add(nameof(Usuario.DataFimSuspensao), usuario.DataFimSuspensao);
        }

        if (usuario.MotivoSuspensao != null)
        {
            sqlSet.Add($"MotivoSuspensao = @{nameof(Usuario.MotivoSuspensao)}");
            parameters.Add(nameof(Usuario.MotivoSuspensao), usuario.MotivoSuspensao);
        }

        if (usuario.IdRespSuspensao != null)
        {
            sqlSet.Add($"IdRespSuspensao = @{nameof(Usuario.IdRespSuspensao)}");
            parameters.Add(nameof(Usuario.IdRespSuspensao), usuario.IdRespSuspensao);
        }

        if (usuario.NomeRespSuspensao != null)
        {
            sqlSet.Add($"NomeRespSuspensao = @{nameof(Usuario.NomeRespSuspensao)}");
            parameters.Add(nameof(Usuario.NomeRespSuspensao), usuario.NomeRespSuspensao);
        }

        if (usuario.DataSuspensao != null)
        {
            sqlSet.Add($"DataSuspensao = @{nameof(Usuario.DataSuspensao)}");
            parameters.Add(nameof(Usuario.DataSuspensao), usuario.DataSuspensao);
        }

        if (usuario.StatusUsuario == StatusUsuarioEnum.Ativo)
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

            sqlSet.Add($"DataInativacao = @{nameof(Usuario.DataInativacao)}");
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

        if (usuario.DataUltimoLogin != null)
        {
            sqlSet.Add($"DataUltimoLogin = @{nameof(Usuario.DataUltimoLogin)}");
            parameters.Add(nameof(Usuario.DataUltimoLogin), usuario.DataUltimoLogin);
        }

        parameters.Add(nameof(Usuario.Id), usuario.Id);

        if (sqlSet.Any())
        {
            string sql = $"UPDATE Usuarios SET {string.Join(", ", sqlSet)} WHERE Id = @{nameof(Usuario.Id)}";
            await _vmiDbContext.Connection.ExecuteAsync(sql, parameters);
        }
    }

    public async Task DeletarUsuario(Guid id)
    {
        string sql = "DELETE FROM Usuarios WHERE Id = @Id";
        await _vmiDbContext.Connection.ExecuteAsync(sql, new { Id = id });
    }

    public bool Save()
    {
        return true;
    }

    public async Task AtualizarFotoPerfil(Guid id, string fotoPerfil)
    {
        const string sql = "UPDATE Usuarios SET FotoPerfil = @FotoPerfil WHERE Id = @Id";
        
        await _vmiDbContext.Connection.ExecuteAsync(sql, new 
        { 
            Id = id, 
            FotoPerfil = fotoPerfil 
        });
    }

    public async Task RemoverFotoPerfil(Guid id)
    {
        const string sql = "UPDATE Usuarios SET FotoPerfil = NULL WHERE Id = @Id";
        
        await _vmiDbContext.Connection.ExecuteAsync(sql, new { Id = id });
    }
}