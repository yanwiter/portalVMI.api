using Vmi.Portal.Entities;

namespace Vmi.Portal.Repositories.Interfaces;

public interface IContatoRepository
{
    Task<Contato> GetByIdAsync(Guid id);
    Task<IEnumerable<Contato>> GetByIdClienteFornecedorAsync(List<Guid> idClienteFornecedor);
    Task AddAsync(Contato contato);
    Task UpdateAsync(Contato contato);
    Task DeleteAsync(Guid id);
}
