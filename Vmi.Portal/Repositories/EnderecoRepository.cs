using Dapper;
using System.Data;
using Vmi.Portal.DbContext;
using Vmi.Portal.Entities;
using Vmi.Portal.Repositories.Interfaces;

namespace Vmi.Portal.Repositories;

public class EnderecoRepository : IEnderecoRepository
{
    private readonly VmiDbContext _vmiDbContext;

    public EnderecoRepository(VmiDbContext vmiDbContext)
    {
        _vmiDbContext = vmiDbContext;
    }

    public async Task<Endereco> GetByIdAsync(Guid id)
    {
        const string sql = @"
                        SELECT 
                            Id,
                            IdClienteFornecedor,
                            Cep,
                            Logradouro,
                            TipoEndereco,
                            Complemento,
                            Numero,
                            Cidade,
                            Bairro,
                            Uf,
                            Referencia
                        FROM Enderecos 
                        WHERE Id = @Id";

        return await _vmiDbContext.Connection.QueryFirstOrDefaultAsync<Endereco>(sql, new { Id = id });
    }

    public async Task<IEnumerable<Endereco>> GetByIdClienteFornecedorAsync(List<Guid> idClienteFornecedor)
    {
        const string sql = @"
        SELECT 
            Id,
            IdClienteFornecedor,
            Cep,
            Logradouro,
            TipoEndereco,
            Complemento,
            Numero,
            Cidade,
            Bairro,
            Uf,
            Referencia
        FROM Enderecos 
        WHERE IdClienteFornecedor IN @IdClienteFornecedor";

        return await _vmiDbContext.Connection.QueryAsync<Endereco>(sql, new { IdClienteFornecedor = idClienteFornecedor });
    }

    public async Task AddAsync(Endereco endereco)
    {
        const string sql = @"
            INSERT INTO Enderecos 
                (Id, IdClienteFornecedor, Cep, Logradouro, TipoEndereco, Complemento, 
                 Numero, Cidade, Bairro, Uf, Referencia)
            VALUES 
                (@Id, @IdClienteFornecedor, @Cep, @Logradouro, @TipoEndereco, @Complemento, 
                 @Numero, @Cidade, @Bairro, @Uf, @Referencia)";

        await _vmiDbContext.Connection.ExecuteAsync(sql, endereco);
    }

    public async Task UpdateAsync(Endereco endereco)
    {
        const string sql = @"
            UPDATE Enderecos SET
                IdClienteFornecedor = @IdClienteFornecedor,
                Cep = @Cep,
                Logradouro = @Logradouro,
                TipoEndereco = @TipoEndereco,
                Complemento = @Complemento,
                Numero = @Numero,
                Cidade = @Cidade,
                Bairro = @Bairro,
                Uf = @Uf,
                Referencia = @Referencia
            WHERE Id = @Id";

        await _vmiDbContext.Connection.ExecuteAsync(sql, endereco);
    }

    public async Task DeleteAsync(Guid id)
    {
        const string sql = @"
            DELETE FROM Enderecos 
            WHERE Id = @Id";

        await _vmiDbContext.Connection.ExecuteAsync(sql, new { Id = id });
    }
}