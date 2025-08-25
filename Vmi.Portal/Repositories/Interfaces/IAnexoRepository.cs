using Vmi.Portal.Entities;

namespace Vmi.Portal.Repositories.Interfaces;
public interface IAnexoRepository
{
    Task<Anexo> GetByIdAsync(Guid id);
    Task<IEnumerable<Anexo>> GetByIdClienteFornecedorAsync(List<Guid> idClienteFornecedor);
    Task<IEnumerable<Anexo>> GetAllAsync();
    Task<Anexo> AddAsync(Anexo anexo);
}
