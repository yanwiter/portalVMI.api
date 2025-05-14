using Vmi.Portal.Entities;

namespace Vmi.Portal.Repositories.Interfaces;

public interface IModuloRepository
{
    IEnumerable<Modulo> ObterTodosModulos();
    Modulo ObterModuloPorId(int id);
    Task AdicionarModulo(Modulo modulo);
    Task AtualizarModulo(Modulo modulo);
    Task RemoverModulo(int id);
    bool Save();
}
