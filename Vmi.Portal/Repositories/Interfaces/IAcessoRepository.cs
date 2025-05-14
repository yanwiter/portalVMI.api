using Vmi.Portal.Entities;

namespace Vmi.Portal.Repositories.Interfaces;

public interface IAcessoRepository
{
    IEnumerable<Acesso> ObterTodosAcessos();
    Acesso ObterAcessoPorId(int id);
    Task AdicionarAcesso(Acesso acesso);
    Task AtualizarAcesso(Acesso acesso);
    Task RemoverAcesso(int id);
    bool Save();
}
