using Dapper;
using System.Text;
using Vmi.Portal.Common;
using Vmi.Portal.DbContext;
using Vmi.Portal.Entities;
using Vmi.Portal.Repositories.Interfaces;
using Vmi.Portal.Enums;

namespace Vmi.Portal.Repositories;

public class PerfilRepository : IPerfilRepository
{
    private readonly VmiDbContext _vmiDbContext;

    public PerfilRepository(VmiDbContext vmiDbContext)
    {
        _vmiDbContext = vmiDbContext;
    }

    public async Task<PagedResult<Perfil>> ObterTodosPerfis(int pageNumber, int pageSize, string nome, StatusPerfilEnum? statusPerfil)
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
                            JustificativaInativacao {nameof(Perfil.JustificativaInativacao)},
                            TipoSuspensao {nameof(Perfil.TipoSuspensao)},
                            DataInicioSuspensao {nameof(Perfil.DataInicioSuspensao)},
                            DataFimSuspensao {nameof(Perfil.DataFimSuspensao)},
                            MotivoSuspensao {nameof(Perfil.MotivoSuspensao)},
                            IdRespSuspensao {nameof(Perfil.IdRespSuspensao)},
                            NomeRespSuspensao {nameof(Perfil.NomeRespSuspensao)},
                            DataSuspensao {nameof(Perfil.DataSuspensao)}
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

    public async Task<Perfil> ObterPerfilPorId(Guid id)
    {
        try
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
                            JustificativaInativacao {nameof(Perfil.JustificativaInativacao)},
                            TipoSuspensao {nameof(Perfil.TipoSuspensao)},
                            DataInicioSuspensao {nameof(Perfil.DataInicioSuspensao)},
                            DataFimSuspensao {nameof(Perfil.DataFimSuspensao)},
                            MotivoSuspensao {nameof(Perfil.MotivoSuspensao)},
                            IdRespSuspensao {nameof(Perfil.IdRespSuspensao)},
                            NomeRespSuspensao {nameof(Perfil.NomeRespSuspensao)},
                            DataSuspensao {nameof(Perfil.DataSuspensao)}
                        FROM
                            Perfis
                        WHERE
                            Id = @Id ";

        return await _vmiDbContext.Connection.
            QueryFirstOrDefaultAsync<Perfil>(sql, new { Id = id });
        }
        catch (Exception ex)
        {

            throw;
        }
    }

    public async Task<Guid> AdicionarPerfil(Perfil perfil)
    {
        string sql = $@"
        INSERT INTO Perfis (
            Id,
            Nome,
            Descricao,
            StatusPerfil,
            IdRespInclusao,
            NomeRespInclusao,
            DataInclusao,
            TipoSuspensao,
            DataInicioSuspensao,
            DataFimSuspensao,
            MotivoSuspensao,
            IdRespSuspensao,
            NomeRespSuspensao,
            DataSuspensao
        )
        VALUES (
            @Id,
            @Nome,
            @Descricao,
            @StatusPerfil,
            @IdRespInclusao,
            @NomeRespInclusao,
            @DataInclusao,
            @TipoSuspensao,
            @DataInicioSuspensao,
            @DataFimSuspensao,
            @MotivoSuspensao,
            @IdRespSuspensao,
            @NomeRespSuspensao,
            @DataSuspensao
        )";

        await _vmiDbContext.Connection.ExecuteAsync(sql, perfil);
        return perfil.Id;
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

        sqlSet.Add($"StatusPerfil = @{nameof(Perfil.StatusPerfil)}");
        parameters.Add(nameof(Perfil.StatusPerfil), perfil.StatusPerfil);

        if (perfil.StatusPerfil == StatusPerfilEnum.Inativo)
        {
            sqlSet.Add($"DataInativacao = @{nameof(Perfil.DataInativacao)}");
            parameters.Add(nameof(Perfil.DataInativacao), DateTime.Now);

            if (perfil.IdRespInativacao != null)
            {
                sqlSet.Add($"IdRespInativacao = @{nameof(Perfil.IdRespInativacao)}");
                parameters.Add(nameof(Perfil.IdRespInativacao), perfil.IdRespInativacao);

                sqlSet.Add($"NomeRespInativacao = @{nameof(Perfil.NomeRespInativacao)}");
                parameters.Add(nameof(Perfil.NomeRespInativacao), perfil.NomeRespInativacao);
            }

            if (perfil.JustificativaInativacao != null)
            {
                sqlSet.Add($"JustificativaInativacao = @{nameof(Perfil.JustificativaInativacao)}");
                parameters.Add(nameof(Perfil.JustificativaInativacao), perfil.JustificativaInativacao);
            }

            sqlSet.Add($"TipoSuspensao = 0");
            sqlSet.Add($"DataInicioSuspensao = NULL");
            sqlSet.Add($"DataFimSuspensao = NULL");
            sqlSet.Add($"MotivoSuspensao = NULL");
            sqlSet.Add($"IdRespSuspensao = NULL");
            sqlSet.Add($"NomeRespSuspensao = NULL");
            sqlSet.Add($"DataSuspensao = NULL");
        }

        else if (perfil.StatusPerfil == StatusPerfilEnum.Suspenso)
        {
            sqlSet.Add($"DataInativacao = NULL");
            sqlSet.Add($"IdRespInativacao = NULL");
            sqlSet.Add($"NomeRespInativacao = NULL");
            sqlSet.Add($"JustificativaInativacao = NULL");

            sqlSet.Add($"TipoSuspensao = @{nameof(Perfil.TipoSuspensao)}");
            parameters.Add(nameof(Perfil.TipoSuspensao), perfil.TipoSuspensao);

            sqlSet.Add($"DataInicioSuspensao = @{nameof(Perfil.DataInicioSuspensao)}");
            parameters.Add(nameof(Perfil.DataInicioSuspensao), perfil.DataInicioSuspensao);

            if (perfil.TipoSuspensao == TipoSuspensaoEnum.Temporaria)
            {
                sqlSet.Add($"DataFimSuspensao = @{nameof(Perfil.DataFimSuspensao)}");
                parameters.Add(nameof(Perfil.DataFimSuspensao), perfil.DataFimSuspensao);
            }
            else
            {
                sqlSet.Add($"DataFimSuspensao = NULL");
            }

            sqlSet.Add($"MotivoSuspensao = @{nameof(Perfil.MotivoSuspensao)}");
            parameters.Add(nameof(Perfil.MotivoSuspensao), perfil.MotivoSuspensao);

            if (perfil.IdRespSuspensao != null)
            {
                sqlSet.Add($"IdRespSuspensao = @{nameof(Perfil.IdRespSuspensao)}");
                parameters.Add(nameof(Perfil.IdRespSuspensao), perfil.IdRespSuspensao);

                sqlSet.Add($"NomeRespSuspensao = @{nameof(Perfil.NomeRespSuspensao)}");
                parameters.Add(nameof(Perfil.NomeRespSuspensao), perfil.NomeRespSuspensao);
            }

            sqlSet.Add($"DataSuspensao = @{nameof(Perfil.DataSuspensao)}");
            parameters.Add(nameof(Perfil.DataSuspensao), DateTime.Now);
        }

        else if (perfil.StatusPerfil == StatusPerfilEnum.Ativo)
        {
            sqlSet.Add($"DataInativacao = NULL");
            sqlSet.Add($"IdRespInativacao = NULL");
            sqlSet.Add($"NomeRespInativacao = NULL");
            sqlSet.Add($"JustificativaInativacao = NULL");
            sqlSet.Add($"TipoSuspensao = 0");
            sqlSet.Add($"DataInicioSuspensao = NULL");
            sqlSet.Add($"DataFimSuspensao = NULL");
            sqlSet.Add($"MotivoSuspensao = NULL");
            sqlSet.Add($"IdRespSuspensao = NULL");
            sqlSet.Add($"NomeRespSuspensao = NULL");
            sqlSet.Add($"DataSuspensao = NULL");
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

    public async Task RemoverPerfil(Guid id)
    {
        string sqlRotinas = "DELETE FROM PerfisRotinas WHERE IdPerfil = @Id";
        await _vmiDbContext.Connection.ExecuteAsync(sqlRotinas, new { Id = id });

        string sql = "DELETE FROM Perfis WHERE Id = @Id";
        await _vmiDbContext.Connection.ExecuteAsync(sql, new { Id = id });
    }

    public async Task<List<Perfil>> ObterPerfisSuspensos()
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
                JustificativaInativacao {nameof(Perfil.JustificativaInativacao)},
                TipoSuspensao {nameof(Perfil.TipoSuspensao)},
                DataInicioSuspensao {nameof(Perfil.DataInicioSuspensao)},
                DataFimSuspensao {nameof(Perfil.DataFimSuspensao)},
                MotivoSuspensao {nameof(Perfil.MotivoSuspensao)},
                IdRespSuspensao {nameof(Perfil.IdRespSuspensao)},
                NomeRespSuspensao {nameof(Perfil.NomeRespSuspensao)},
                DataSuspensao {nameof(Perfil.DataSuspensao)}
            FROM
                Perfis WITH(NOLOCK)
            WHERE
                StatusPerfil = @StatusPerfil
            ORDER BY
                Nome";

        var perfis = await _vmiDbContext.Connection.QueryAsync<Perfil>(sql, 
            new { StatusPerfil = StatusPerfilEnum.Suspenso });
        
        return perfis.ToList();
    }

    public async Task<List<Perfil>> ObterPerfisSuspensosTemporarios()
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
                JustificativaInativacao {nameof(Perfil.JustificativaInativacao)},
                TipoSuspensao {nameof(Perfil.TipoSuspensao)},
                DataInicioSuspensao {nameof(Perfil.DataInicioSuspensao)},
                DataFimSuspensao {nameof(Perfil.DataFimSuspensao)},
                MotivoSuspensao {nameof(Perfil.MotivoSuspensao)},
                IdRespSuspensao {nameof(Perfil.IdRespSuspensao)},
                NomeRespSuspensao {nameof(Perfil.NomeRespSuspensao)},
                DataSuspensao {nameof(Perfil.DataSuspensao)}
            FROM
                Perfis WITH(NOLOCK)
            WHERE
                StatusPerfil = @StatusPerfil
                AND TipoSuspensao = @TipoSuspensao
            ORDER BY
                Nome";

        var perfis = await _vmiDbContext.Connection.QueryAsync<Perfil>(sql, 
            new { 
                StatusPerfil = StatusPerfilEnum.Suspenso, 
                TipoSuspensao = TipoSuspensaoEnum.Temporaria 
            });
        
        return perfis.ToList();
    }

    public async Task<List<Perfil>> ObterPerfisSuspensosPermanentes()
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
                JustificativaInativacao {nameof(Perfil.JustificativaInativacao)},
                TipoSuspensao {nameof(Perfil.TipoSuspensao)},
                DataInicioSuspensao {nameof(Perfil.DataInicioSuspensao)},
                DataFimSuspensao {nameof(Perfil.DataFimSuspensao)},
                MotivoSuspensao {nameof(Perfil.MotivoSuspensao)},
                IdRespSuspensao {nameof(Perfil.IdRespSuspensao)},
                NomeRespSuspensao {nameof(Perfil.NomeRespSuspensao)},
                DataSuspensao {nameof(Perfil.DataSuspensao)}
            FROM
                Perfis WITH(NOLOCK)
            WHERE
                StatusPerfil = @StatusPerfil
                AND TipoSuspensao = @TipoSuspensao
            ORDER BY
                Nome";

        var perfis = await _vmiDbContext.Connection.QueryAsync<Perfil>(sql, 
            new { 
                StatusPerfil = StatusPerfilEnum.Suspenso, 
                TipoSuspensao = TipoSuspensaoEnum.Permanente 
            });
        
        return perfis.ToList();
    }
}