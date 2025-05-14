using Dapper;
using System.Text;
using Vmi.Portal.Common;
using Vmi.Portal.DbContext;
using Vmi.Portal.Entities;
using Vmi.Portal.Repositories.Interfaces;

namespace Vmi.Portal.Repositories;

public class PerfilRepository : IPerfilRepository
{
    private readonly VmiDbContext _vmiDbContext;

    public PerfilRepository(VmiDbContext vmiDbContext)
    {
        _vmiDbContext = vmiDbContext;
    }

    public async Task<PagedResult<Perfil>> ObterTodosPerfis(int pageNumber, int pageSize, string nome, bool? statusPerfil)
    {
        string sql = $@"
                        SELECT
                            Id {nameof(Perfil.Id)},
                            Nome {nameof(Perfil.Nome)},
                            Descricao {nameof(Perfil.Descricao)},
                            StatusPerfil {nameof(Perfil.StatusPerfil)},
                            DataInclusao {nameof(Perfil.DataInclusao)},
                            IdRespInclusao {nameof(Perfil.IdRespInclusao)},
                            NomeRespInclusao {nameof(Perfil.NomeRespInclusao)},
                            IdRespUltimaModificacao {nameof(Perfil.IdRespUltimaModificacao)},
                            NomeRespUltimaModificacao {nameof(Perfil.NomeRespUltimaModificacao)},
                            DataUltimaModificacao {nameof(Perfil.DataUltimaModificacao)},
                            JustificativaInativacao {nameof(Perfil.JustificativaInativacao)}
                        FROM
                            Perfis WITH(NOLOCK)
                        WHERE
                            (
                                @NOME IS NULL
                                OR Nome LIKE @NOME
                            )
                            AND (
                                @STATUS_PERFIL IS NULL
                                OR StatusPerfil = @STATUS_PERFIL
                            )
                        ORDER BY
                            Nome OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

                        SELECT
                            COUNT(*)
                        FROM
                            Perfis WITH(NOLOCK)
                        WHERE
                            (
                                @NOME IS NULL
                                OR Nome LIKE @NOME
                            )
                            AND (
                                @STATUS_PERFIL IS NULL
                                OR StatusPerfil = @STATUS_PERFIL
                            );

                        ";

        using (var multi = await _vmiDbContext.Connection.QueryMultipleAsync(sql,
            new
            {
                NOME = string.IsNullOrEmpty(nome) ? null : $"%{nome}%",
                STATUS_PERFIL = statusPerfil,
                Offset = (pageNumber - 1) * pageSize,
                PageSize = pageSize
            }))
        {
            var perfis = await multi.ReadAsync<Perfil>();
            var totalCount = await multi.ReadFirstAsync<int>();

            return new PagedResult<Perfil>
            {
                Items = perfis,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }

    public async Task<Perfil> ObterPerfilPorId(int id)
    {
        string sql = $@"
                        SELECT
                            Id {nameof(Perfil.Id)},
                            Nome {nameof(Perfil.Nome)},
                            Descricao {nameof(Perfil.Descricao)},
                            StatusPerfil {nameof(Perfil.StatusPerfil)},
                            DataInclusao {nameof(Perfil.DataInclusao)},
                            IdRespInclusao {nameof(Perfil.IdRespInclusao)},
                            NomeRespInclusao {nameof(Perfil.NomeRespInclusao)},
                            IdRespUltimaModificacao {nameof(Perfil.IdRespUltimaModificacao)},
                            NomeRespUltimaModificacao {nameof(Perfil.NomeRespUltimaModificacao)},
                            DataUltimaModificacao {nameof(Perfil.DataUltimaModificacao)},
                            JustificativaInativacao {nameof(Perfil.JustificativaInativacao)}
                        FROM
                            Perfis
                        WHERE
                            Id = @Id ";

        return await _vmiDbContext.Connection.
            QueryFirstOrDefaultAsync<Perfil>(sql, new { Id = id });
    }

    public async Task<int> AdicionarPerfil(Perfil perfil)
    {
        string sql = $@"
                        INSERT INTO
                            Perfis (
                                {nameof(Perfil.Nome)},
                                {nameof(Perfil.Descricao)},
                                {nameof(Perfil.StatusPerfil)},
                                {nameof(Perfil.IdRespInclusao)},
                                {nameof(Perfil.NomeRespInclusao)},
                                {nameof(Perfil.DataInclusao)}
                            )
                        VALUES
                            (
                                @Nome,
                                @Descricao,
                                @StatusPerfil,
                                @IdRespInclusao,
                                @NomeRespInclusao,
                                @DataInclusao
                            );

                        SELECT
                            SCOPE_IDENTITY();
                        ";

        return await _vmiDbContext.Connection.ExecuteScalarAsync<int>(sql, perfil);
    }

    public async Task AtualizarPerfil(Perfil perfil)
    {
        var parameters = new DynamicParameters();
        var sqlSet = new List<string>();

        if (perfil.Nome != null)
        {
            sqlSet.Add($"Nome = @{nameof(Perfil.Nome)}");
            parameters.Add(nameof(Perfil.Nome), perfil.Nome);
        }

        if (perfil.Descricao != null)
        {
            sqlSet.Add($"Descricao = @{nameof(Perfil.Descricao)}");
            parameters.Add(nameof(Perfil.Descricao), perfil.Descricao);
        }

        if (perfil.StatusPerfil)
        {
            sqlSet.Add($"StatusPerfil = @{nameof(Perfil.StatusPerfil)}");
            parameters.Add(nameof(Perfil.StatusPerfil), perfil.StatusPerfil);

            sqlSet.Add($"DataInativacao = NULL");
            sqlSet.Add($"IdRespInativacao = NULL");
            sqlSet.Add($"NomeRespInativacao = NULL");
            sqlSet.Add($"JustificativaInativacao = NULL");
        }
        else
        {
            sqlSet.Add($"StatusPerfil = @{nameof(Perfil.StatusPerfil)}");
            parameters.Add(nameof(Perfil.StatusPerfil), perfil.StatusPerfil);

            if (perfil.IdRespInativacao != null)
            {
                sqlSet.Add($"IdRespInativacao = @{nameof(Perfil.IdRespInativacao)}");
                parameters.Add(nameof(Perfil.IdRespInativacao), perfil.IdRespInativacao);

                sqlSet.Add($"NomeRespInativacao = @{nameof(Perfil.NomeRespInativacao)}");
                parameters.Add(nameof(Perfil.NomeRespInativacao), perfil.NomeRespInativacao);
            }

            sqlSet.Add($"DataInativacao = @{nameof(Perfil.DataInativacao)}");
            parameters.Add(nameof(Perfil.DataInativacao), DateTime.Now);

            if (perfil.JustificativaInativacao != null)
            {
                sqlSet.Add($"JustificativaInativacao = @{nameof(Perfil.JustificativaInativacao)}");
                parameters.Add(nameof(Perfil.JustificativaInativacao), perfil.JustificativaInativacao);
            }
        }

        if (perfil.IdRespUltimaModificacao != null)
        {
            sqlSet.Add($"IdRespUltimaModificacao = @{nameof(Perfil.IdRespUltimaModificacao)}");
            parameters.Add(nameof(Perfil.IdRespUltimaModificacao), perfil.IdRespUltimaModificacao);
        }

        if (perfil.NomeRespUltimaModificacao != null)
        {
            sqlSet.Add($"NomeRespUltimaModificacao = @{nameof(Perfil.NomeRespUltimaModificacao)}");
            parameters.Add(nameof(Perfil.NomeRespUltimaModificacao), perfil.NomeRespUltimaModificacao);
        }

        if (perfil.DataUltimaModificacao != null)
        {
            sqlSet.Add($"DataUltimaModificacao = @{nameof(Perfil.DataUltimaModificacao)}");
            parameters.Add(nameof(Perfil.DataUltimaModificacao), perfil.DataUltimaModificacao);
        }

        parameters.Add(nameof(Perfil.Id), perfil.Id);

        if (sqlSet.Any())
        {
            string sql = $"UPDATE Perfis SET {string.Join(", ", sqlSet)} WHERE Id = @{nameof(Perfil.Id)}";
            await _vmiDbContext.Connection.ExecuteAsync(sql, parameters);
        }
    }

    public async Task RemoverPerfil(int id)
    {
        string sqlRotinas = "DELETE FROM PerfisRotinas WHERE Perfil_id = @Id";
        await _vmiDbContext.Connection.ExecuteAsync(sqlRotinas, new { Id = id });

        string sql = "DELETE FROM Perfis WHERE Id = @Id";
        await _vmiDbContext.Connection.ExecuteAsync(sql, new { Id = id });
    }
}