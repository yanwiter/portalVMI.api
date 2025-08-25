using Vmi.Portal.Entities;

namespace Vmi.Portal.Repositories.Interfaces;

public interface IEnderecoRepository
{
    Task<Endereco> GetByIdAsync(Guid id);
    Task<IEnumerable<Endereco>> GetByIdClienteFornecedorAsync(List<Guid> idClienteFornecedor);
    Task AddAsync(Endereco endereco);
    Task UpdateAsync(Endereco endereco);
    Task DeleteAsync(Guid id);
}
