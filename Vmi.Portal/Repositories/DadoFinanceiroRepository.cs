using Dapper;
using Vmi.Portal.DbContext;
using Vmi.Portal.Entities;
using Vmi.Portal.Repositories.Interfaces;

namespace Vmi.Portal.Repositories;

public class DadoFinanceiroRepository : IDadoFinanceiroRepository
{
    private readonly VmiDbContext _vmiDbContext;

    public DadoFinanceiroRepository(VmiDbContext vmiDbContext)
    {
        _vmiDbContext = vmiDbContext;
    }
    public async Task<DadoFinanceiro> GetByIdAsync(Guid id)
    {
        const string sql = @"
                SELECT * FROM DadosFinanceiros 
                WHERE Id = @Id";

        return await _vmiDbContext.Connection.QueryFirstOrDefaultAsync<DadoFinanceiro>(sql, new { Id = id });
    }

    public async Task<IEnumerable<DadoFinanceiro>> GetByIdClienteFornecedorAsync(List<Guid> idClienteFornecedor)
    {
        const string sql = @"
           SELECT 
                Id,
                IdClienteFornecedor,
                LimiteCredito,
                CondicaoPagamento,
                Banco,
                Agencia,
                Conta,
                TipoConta,
                ChavePix
            FROM DadosFinanceiros 
            WHERE IdClienteFornecedor IN @IdClienteFornecedor";

        return await _vmiDbContext.Connection.QueryAsync<DadoFinanceiro>(sql, new { IdClienteFornecedor = idClienteFornecedor });
    }

    public async Task<IEnumerable<DadoFinanceiro>> GetAllAsync()
    {
        const string sql = "SELECT * FROM DadosFinanceiros";
        return await _vmiDbContext.Connection.QueryAsync<DadoFinanceiro>(sql);
    }

    public async Task AddAsync(DadoFinanceiro dadoFinanceiro)
    {
        const string sql = @"
                INSERT INTO DadosFinanceiros 
                (Id, ClienteFornecedorId, LimiteCredito, CondicaoPagamento, Banco, Agencia, Conta, TipoConta, ChavePix)
                VALUES 
                (@Id, @ClienteFornecedorId, @LimiteCredito, @CondicaoPagamento, @Banco, @Agencia, @Conta, @TipoConta, @ChavePix)";

        await _vmiDbContext.Connection.ExecuteAsync(sql, dadoFinanceiro);
    }

    public async Task UpdateAsync(DadoFinanceiro dadoFinanceiro)
    {
        const string sql = @"
             UPDATE 
                DadosFinanceiros 
                    SET
                        ClienteFornecedorId = @ClienteFornecedorId,
                        LimiteCredito = @LimiteCredito,
                        CondicaoPagamento = @CondicaoPagamento,
                        Banco = @Banco,
                        Agencia = @Agencia,
                        Conta = @Conta,
                        TipoConta = @TipoConta,
                        ChavePix = @ChavePix
                WHERE Id = @Id";

        await _vmiDbContext.Connection.ExecuteAsync(sql, dadoFinanceiro);
    }

    public async Task DeleteAsync(Guid id)
    {
        const string sql = "DELETE FROM DadosFinanceiros WHERE Id = @Id";
        await _vmiDbContext.Connection.ExecuteAsync(sql, new { Id = id });
    }
}
