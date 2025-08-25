using Vmi.Portal.Entities;

namespace Vmi.Portal.Repositories.Interfaces;

public interface IPerfilRotinaRepository
{
    Task<IEnumerable<PerfilRotina>> ObterPerfilRotinaPorIdPerfil(Guid idPerfil);
    Task InserirPerfilRotinas(IEnumerable<PerfilRotina> perfilRotinas);
    Task ExcluirPerfilRotina(Guid idPerfil);
}
