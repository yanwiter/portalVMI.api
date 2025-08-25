using Vmi.Portal.Common;
using Vmi.Portal.Entities;
using Vmi.Portal.Enums;

namespace Vmi.Portal.Repositories.Interfaces;

public interface IPerfilRepository
{
    Task<PagedResult<Perfil>> ObterTodosPerfis(int pageNumber, int pageSize, string nome, StatusPerfilEnum? statusPerfil);
    Task<Perfil> ObterPerfilPorId(Guid id);
    Task<Guid> AdicionarPerfil(Perfil perfil);
    Task AtualizarPerfil(Perfil perfil);
    Task RemoverPerfil(Guid id);
    Task<List<Perfil>> ObterPerfisSuspensos();
    Task<List<Perfil>> ObterPerfisSuspensosTemporarios();
    Task<List<Perfil>> ObterPerfisSuspensosPermanentes();
}
