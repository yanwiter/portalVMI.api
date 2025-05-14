using Vmi.Portal.Entities;

namespace Vmi.Portal.Repositories.Interfaces;

public interface IPerfilRotinaRepository
{
    Task<IEnumerable<PerfilRotina>> ObterPerfilRotinaPorIdPerfil(int idPerfil);
    Task InserirPerfilRotinas(IEnumerable<PerfilRotina> perfilRotinas);
    Task ExcluirPerfilRotina(int idPerfil);
}
