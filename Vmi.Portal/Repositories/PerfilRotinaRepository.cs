using Dapper;
using Vmi.Portal.DbContext;
using Vmi.Portal.Entities;
using Vmi.Portal.Repositories.Interfaces;

namespace Vmi.Portal.Repositories;

public class PerfilRotinaRepository(VmiDbContext vmiDbContext) : IPerfilRotinaRepository
{
    public async Task<IEnumerable<PerfilRotina>> ObterPerfilRotinaPorIdPerfil(Guid idPerfil)
    {
        string sql = $@"
                        SELECT
                            Id {nameof(PerfilRotina.Id)},
                            IdPerfil {nameof(PerfilRotina.IdPerfil)},
                            IdRotina {nameof(PerfilRotina.IdRotina)},
                            IdAcesso {nameof(PerfilRotina.IdAcesso)}
                        FROM
                            PerfisRotinas
                        WHERE
                            IdPerfil = @IDPERFIL;
                    ";

        return await vmiDbContext.Connection.QueryAsync<PerfilRotina>(
            sql,
            new { IDPERFIL = idPerfil.ToString() }
            );
    }

    public async Task InserirPerfilRotinas(IEnumerable<PerfilRotina> perfilRotinas)
    {
        string sql = $@"
                        INSERT INTO
                            PerfisRotinas (
                                IdPerfil,
                                IdRotina,
                                IdAcesso
                            )
                        VALUES
                            (
                                @{nameof(PerfilRotina.IdPerfil)},
                                @{nameof(PerfilRotina.IdRotina)},
                                @{nameof(PerfilRotina.IdAcesso)}
                            )
                    ";

        await vmiDbContext.Connection.ExecuteAsync(
            sql,
            perfilRotinas);
    }

    public async Task ExcluirPerfilRotina(Guid idPerfil)
    {
        string sql = $@"
                        DELETE FROM
                            PerfisRotinas
                        WHERE
                            IdPerfil = @IDPERFIL
                    ";

        await vmiDbContext.Connection.ExecuteAsync(sql, new { IDPERFIL = idPerfil });
    }
}
