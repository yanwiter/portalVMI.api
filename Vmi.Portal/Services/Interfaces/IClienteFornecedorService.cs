using Vmi.Portal.Common;
using Vmi.Portal.Entities;

namespace Vmi.Portal.Services.Interfaces
{
    public interface IClienteFornecedorService
    {
        Task<PagedResult<ClienteFornecedor>> BuscarTodos(
            int pageNumber,
            int pageSize,
            string razaoSocial = null,
            string nomeFantasia = null,
            string cpfCnpj = null,
            int? tipoCadastro = null,
            int? tipoPessoa = null,
            bool? statusCadastro = null);

        Task<byte[]> ExportarClientesFornecedoresParaExcel(int pageNumber,
            int pageSize,
            string razaoSocial = null,
            string nomeFantasia = null,
            string cpfCnpj = null,
            int? tipoCadastro = null,
            int? tipoPessoa = null,
            bool? statusCadastro = null);

        Task<ClienteFornecedor> BuscarPorId(Guid id);
        Task<ClienteFornecedor> Adicionar(ClienteFornecedor clienteFornecedor);
        Task<ClienteFornecedor> Atualizar(ClienteFornecedor clienteFornecedor);
        Task Remover(Guid id);
    }
}
