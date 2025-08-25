using Vmi.Portal.Common;
using Vmi.Portal.Entities;
using Vmi.Portal.Models;
using Vmi.Portal.Enums;

namespace Vmi.Portal.Services;

public interface IPerfilService
{
    Task<PagedResult<Perfil>> ObterTodosPerfis(int pageNumber, int pageSize, string nome, StatusPerfilEnum? statusPerfil);
    Task<InformacaoPerfil> ObterTodosAcessoPerfis();
    Task<Perfil> ObterPerfilPorId(Guid id);
    Task AdicionarPerfil(Perfil perfil);
    Task AtualizarPerfil(Perfil perfil, Guid usuarioId);
    Task RemoverPerfil(Guid id);
    Task<byte[]> ExportarPerfisParaExcel(int pageNumber, int pageSize, string nome = null, StatusPerfilEnum? statusPerfil = null);
    Task<Result<InformacaoPerfil>> ObterInformacoesDoPerfil(Guid id, Guid acesso, string rotina, string modulo);
    Task<Perfil> SalvarPerfil(InformacaoPerfil informacaoPerfil);
    Task AtualizarPerfil(InformacaoPerfil informacaoPerfil);
    Task VerificarSuspensoesExpiradas();
    Task<List<Perfil>> ObterPerfisSuspensos();
    Task<List<Perfil>> ObterPerfisSuspensosTemporarios();
    Task<List<Perfil>> ObterPerfisSuspensosPermanentes();
    Task<bool> VerificarSePerfilEstaSuspenso(Guid perfilId);
}