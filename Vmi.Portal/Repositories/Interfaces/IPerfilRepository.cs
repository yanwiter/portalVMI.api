using Vmi.Portal.Common;
using Vmi.Portal.Entities;

namespace Vmi.Portal.Repositories.Interfaces;

public interface IPerfilRepository
{
    Task<PagedResult<Perfil>> ObterTodosPerfis(int pageNumber, int pageSize, string nome, bool? statusPerfil);
    Task<Perfil> ObterPerfilPorId(int id);
    Task<int> AdicionarPerfil(Perfil perfil);
    Task AtualizarPerfil(Perfil perfil);
    Task RemoverPerfil(int id);
}
