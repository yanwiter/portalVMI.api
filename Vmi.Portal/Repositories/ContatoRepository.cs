using Dapper;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.Configuration;
using Vmi.Portal.DbContext;
using Vmi.Portal.Entities;
using Vmi.Portal.Repositories.Interfaces;

namespace Vmi.Portal.Repositories;

public class ContatoRepository : IContatoRepository
{
    private readonly VmiDbContext _vmiDbContext;

    public ContatoRepository(VmiDbContext vmiDbContext)
    {
        _vmiDbContext = vmiDbContext;
    }
    public async Task<Contato> GetByIdAsync(Guid id)
    {
        var sql = @"
                SELECT * FROM Contatos 
                WHERE Id = @Id;
                
                SELECT * FROM ClientesFornecedores 
                WHERE Id = (SELECT ClienteFornecedorId FROM Contatos WHERE Id = @Id);";

        using (var multi = await _vmiDbContext.Connection.QueryMultipleAsync(sql, new { Id = id }))
        {
            var contato = await multi.ReadFirstOrDefaultAsync<Contato>();
            if (contato != null)
            {
                contato.ClienteFornecedor = await multi.ReadFirstOrDefaultAsync<ClienteFornecedor>();
            }
            return contato;
        }
    }

    public async Task<IEnumerable<Contato>> GetByIdClienteFornecedorAsync(List<Guid> idClienteFornecedor)
    {
        var sql = @"
                    SELECT 
                        Id,
                        IdClienteFornecedor,
                        Nome,
                        Cargo,
                        Email,
                        Telefone,
                        Celular,
                        Ramal
                    FROM Contatos
                    WHERE IdClienteFornecedor IN @IdClienteFornecedor";

        return await _vmiDbContext.Connection.QueryAsync<Contato>(sql, new { IdClienteFornecedor = idClienteFornecedor });
    }

    public async Task AddAsync(Contato contato)
    {
        var sql = @"
                INSERT INTO Contatos 
                (Id, IdClienteFornecedor, Nome, Cargo, Email, Telefone, Celular, Ramal)
                VALUES 
                (@Id, @IdClienteFornecedor, @Nome, @Cargo, @Email, @Telefone, @Celular, @Ramal)";

        await _vmiDbContext.Connection.ExecuteAsync(sql, new
        {
            contato.Id,
            contato.IdClienteFornecedor,
            contato.Nome,
            contato.Cargo,
            contato.Email,
            contato.Telefone,
            contato.Celular,
            contato.Ramal
        });
    }

    public async Task UpdateAsync(Contato contato)
    {
        var sql = @"
                UPDATE Contatos SET
                IdClienteFornecedor = @IdClienteFornecedor,
                Nome = @Nome,
                Cargo = @Cargo,
                Email = @Email,
                Telefone = @Telefone,
                Celular = @Celular,
                Ramal = @Ramal
                WHERE Id = @Id";

        await _vmiDbContext.Connection.ExecuteAsync(sql, new
        {
            contato.Id,
            contato.IdClienteFornecedor,
            contato.Nome,
            contato.Cargo,
            contato.Email,
            contato.Telefone,
            contato.Celular,
            contato.Ramal
        });
    }

    public async Task DeleteAsync(Guid id)
    {
        var sql = "DELETE FROM Contatos WHERE Id = @Id";
        await _vmiDbContext.Connection.ExecuteAsync(sql, new { Id = id });
    }

}
