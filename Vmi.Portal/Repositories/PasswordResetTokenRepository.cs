using Dapper;
using Vmi.Portal.Data.Repositories.Interfaces;
using Vmi.Portal.DbContext;
using Vmi.Portal.Entities;

namespace Vmi.Portal.Data.Repositories;
public class PasswordResetTokenRepository : IPasswordResetTokenRepository
{
    private readonly VmiDbContext _vmiDbContext;

    public PasswordResetTokenRepository(VmiDbContext vmiDbContext)
    {
        _vmiDbContext = vmiDbContext;
    }

    public async Task<PasswordResetToken> PegarTokenValidoAsync(string token)
    {
        var sql = @"
                SELECT t.*, u.* 
                FROM TokenRedefinicaoSenha t
                INNER JOIN Usuarios u ON t.UsuarioId = u.Id
                WHERE t.Token = @Token 
                AND t.IsTokenUsado = 0 
                AND t.DataExpiracao > @Now";

        var result = await _vmiDbContext.Connection.QueryAsync<PasswordResetToken, Usuario, PasswordResetToken>(
            sql,
            (tokenObj, usuario) =>
            {
                tokenObj.Usuario = usuario;
                return tokenObj;
            },
            new { Token = token, Now = DateTime.UtcNow },
            splitOn: "Id");

        return result.FirstOrDefault();
    }

    public async Task CriarTokenRedefinicaoSenhaAsync(PasswordResetToken token)
    {
        var sql = @"
                INSERT INTO TokenRedefinicaoSenha 
                (Token, UsuarioId, DataExpiracao, IsTokenUsado)
                VALUES 
                (@Token, @UsuarioId, @DataExpiracao, @IsTokenUsado)";

        await _vmiDbContext.Connection.ExecuteAsync(sql, new
        {
            token.Token,
            token.UsuarioId,
            token.DataExpiracao,
            token.IsTokenUsado
        });
    }

    public async Task AtualizarTokenRedefinicaoSenhaAsync(PasswordResetToken token)
    {
        var sql = @"
                UPDATE TokenRedefinicaoSenha 
                SET IsTokenUsado = @IsTokenUsado, 
                    DataUsoToken = @DataUsoToken
                WHERE Id = @Id";

        await _vmiDbContext.Connection.ExecuteAsync(sql, new
        {
            token.IsTokenUsado,
            token.DataUsoToken,
            token.Id
        });
    }
}