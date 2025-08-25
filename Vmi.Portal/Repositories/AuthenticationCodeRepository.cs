using Dapper;
using System;
using System.Data;
using System.Threading.Tasks;
using Vmi.Portal.Data.Repositories.Interfaces;
using Vmi.Portal.DbContext;
using Vmi.Portal.Entities;
using Vmi.Portal.Repositories.Interfaces;

namespace Vmi.Portal.Repositories;
public class AuthenticationCodeRepository : IAuthenticationCodeRepository
{
    private readonly VmiDbContext _vmiDbContext;

    public AuthenticationCodeRepository(VmiDbContext vmiDbContext)
    {
        _vmiDbContext = vmiDbContext;
    }

    public async Task SaveAuthCodeAsync(Guid idUsuario, string codigo, DateTime dataExpiracao)
    {
        const string sql = @"
        INSERT INTO CodigosAutenticacao (Id, IdUsuario, Codigo, DataExpiracao, DataCriacao)
        VALUES (NEWID(), @IdUsuario, @Codigo, @DataExpiracao, GETUTCDATE())";

        await _vmiDbContext.Connection.ExecuteAsync(sql, new
        {
            IdUsuario = idUsuario,
            Codigo = codigo,
            DataExpiracao = dataExpiracao
        });
    }

    public async Task<bool> ValidateCodeAsync(Guid idUsuario, string codigo)
    {
        const string sql = @"
        SELECT TOP 1 * FROM CodigosAutenticacao 
        WHERE IdUsuario = @IdUsuario AND Codigo = @Codigo
        ORDER BY DataCriacao DESC";

        var authCode = await _vmiDbContext.Connection.QueryFirstOrDefaultAsync<CodigosAutenticacao>(sql, new
        {
            IdUsuario = idUsuario,
            Codigo = codigo
        });

        if (authCode == null || authCode.DataExpiracao < DateTime.UtcNow)
            return false;

        await DeleteCodeAsync(authCode.Id);
        return true;
    }

    public async Task DeleteUserCodesAsync(Guid idUsuario)
    {
        const string sql = "DELETE FROM CodigosAutenticacao WHERE IdUsuario = @IdUsuario";
        await _vmiDbContext.Connection.ExecuteAsync(sql, new { IdUsuario = idUsuario });
    }

    public async Task<int> GetUserCodeCountAsync(Guid idUsuario)
    {
        const string sql = "SELECT COUNT(*) FROM CodigosAutenticacao WHERE IdUsuario = @IdUsuario";
        return await _vmiDbContext.Connection.ExecuteScalarAsync<int>(sql, new { IdUsuario = idUsuario });
    }

    public async Task CleanExpiredCodesAsync()
    {
        const string sql = "DELETE FROM CodigosAutenticacao WHERE DataExpiracao < GETUTCDATE()";
        await _vmiDbContext.Connection.ExecuteAsync(sql);
    }

    private async Task DeleteCodeAsync(Guid id)
    {
        const string sql = "DELETE FROM CodigosAutenticacao WHERE Id = @Id";
        await _vmiDbContext.Connection.ExecuteAsync(sql, new { Id = id });
    }
}