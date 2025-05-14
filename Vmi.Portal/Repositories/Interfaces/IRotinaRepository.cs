using Vmi.Portal.Entities;

namespace Vmi.Portal.Repositories.Interfaces;

public interface IRotinaRepository
{
    Task<IEnumerable<Rotina>> ObterRotinas(
        string rotina,
        string modulo);
}
