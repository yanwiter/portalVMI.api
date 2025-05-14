using Dapper;
using Vmi.Portal.DbContext;
using Vmi.Portal.Entities;
using Vmi.Portal.Repositories.Interfaces;

namespace Vmi.Portal.Repositories;

public class RotinaRepository(VmiDbContext vmiDbContext) : IRotinaRepository
{
    public async Task<IEnumerable<Rotina>> ObterRotinas(
        string rotina,
        string modulo)
    {
        string sql = @"
                SELECT
                    r.Id AS Id,
                    r.Nome AS Nome,
                    r.Modulo_id AS ModuloId,
                    m.Nome AS ModuloNome
                FROM
                    Rotinas r
                    LEFT JOIN Modulos m ON r.Modulo_id = m.Id
                WHERE
                    (@ROTINA IS NULL OR r.Nome LIKE @ROTINA)  AND 
                    (@MODULO IS NULL OR m.Nome LIKE @MODULO);";

        return await vmiDbContext.Connection.QueryAsync<Rotina>(
            sql,
            new
            {
                ROTINA = string.IsNullOrEmpty(rotina) ? null : $"%{rotina}%",
                MODULO = string.IsNullOrEmpty(modulo) ? null : $"%{modulo}%"
            });
    }
}