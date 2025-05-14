using Dapper;
using Vmi.Portal.DbContext;
using Vmi.Portal.Entities;
using Vmi.Portal.Repositories.Interfaces;

namespace Vmi.Portal.Repositories;

public class PerfilRotinaRepository(VmiDbContext vmiDbContext) : IPerfilRotinaRepository
{

    public async Task<IEnumerable<PerfilRotina>> ObterPerfilRotinaPorIdPerfil(int idPerfil)
    {
        string sql = $@"
                        SELECT
                            Id {nameof(PerfilRotina.Id)},
                            Perfil_id {nameof(PerfilRotina.PerfilId)},
                            Rotina_id {nameof(PerfilRotina.RotinaId)},
                            Acesso_id {nameof(PerfilRotina.AcessoId)}
                        FROM
                            PerfisRotinas
                        WHERE
                            Perfil_id = @PERFIL_ID;
                    ";

        return await vmiDbContext.Connection.QueryAsync<PerfilRotina>(
            sql,
            new { PERFIL_ID = idPerfil }
            );
    }

    public async Task InserirPerfilRotinas(IEnumerable<PerfilRotina> perfilRotinas)
    {
        string sql = $@"
                        INSERT INTO
                            PerfisRotinas (
                                Perfil_id,
                                Rotina_id,
                                Acesso_id
                            )
                        VALUES
                            (
                                @{nameof(PerfilRotina.PerfilId)},
                                @{nameof(PerfilRotina.RotinaId)},
                                @{nameof(PerfilRotina.AcessoId)}
                            )
                    ";

        await vmiDbContext.Connection.ExecuteAsync(
            sql,
            perfilRotinas);
    }

    public async Task ExcluirPerfilRotina(int idPerfil)
    {
        string sql = $@"
                        DELETE FROM
                            PerfisRotinas
                        WHERE
                            Perfil_id = @PERFIL_ID
                    ";

        await vmiDbContext.Connection.ExecuteAsync(sql, new { PERFIL_ID = idPerfil });
    }
}
