using Vmi.Portal.Entities;

namespace Vmi.Portal.Repositories.Interfaces
{
    public interface IDadoFinanceiroRepository
    {
        Task<DadoFinanceiro> GetByIdAsync(Guid id);
        Task<IEnumerable<DadoFinanceiro>> GetByIdClienteFornecedorAsync(List<Guid> idClienteFornecedor);
        Task AddAsync(DadoFinanceiro dadoFinanceiro);
        Task UpdateAsync(DadoFinanceiro dadoFinanceiro);
        Task DeleteAsync(Guid id);
    }
}
