using Vmi.Portal.Common;
using Vmi.Portal.Entities;
using static Vmi.Portal.Common.Enums;

namespace Vmi.Portal.Repositories.Interfaces;

public interface IClienteFornecedorRepository
{
    Task<PagedResult<ClienteFornecedor>> BuscarTodos(
        int pageNumber,
        int pageSize,
        string razaoSocial,
        string nomeFantasia,
        string cpfCnpj,
        int? tipoCadastro,
        int? tipoPessoa,
        bool? status
    );
    Task<ClienteFornecedor> BuscarPorId(Guid id);
    Task<ClienteFornecedor> Adicionar(ClienteFornecedor clienteFornecedor);
    Task<ClienteFornecedor> Atualizar(ClienteFornecedor clienteFornecedor);
    Task Remover(Guid id);
}
