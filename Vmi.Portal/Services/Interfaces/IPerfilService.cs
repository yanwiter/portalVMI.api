using Vmi.Portal.Common;
using Vmi.Portal.Entities;
using Vmi.Portal.Enums;
using Vmi.Portal.Models;

namespace Vmi.Portal.Services;

public interface IPerfilService
{
    Task<PagedResult<Perfil>> ObterTodosPerfis(int pageNumber, int pageSize, string nome, bool? statusPerfil);
    Task<InformacaoPerfil> ObterTodosAcessoPerfis();
    Task<Perfil> ObterPerfilPorId(int id);
    Task AdicionarPerfil(Perfil perfil);
    Task AtualizarPerfil(Perfil perfil, int usuarioId);
    Task RemoverPerfil(int id);

    Task<Result<InformacaoPerfil>> ObterInformacoesDoPerfil(int id, AcessoEnum acesso, string rotina, string modulo);
    Task<Perfil> SalvarPerfil(InformacaoPerfil informacaoPerfil);
    Task AtualizarPerfil(InformacaoPerfil informacaoPerfil);
}