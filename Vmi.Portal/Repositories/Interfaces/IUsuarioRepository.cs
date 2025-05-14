using System.Collections.Generic;
using Vmi.Portal.Common;
using Vmi.Portal.Entities;

namespace Vmi.Portal.Repositories;

public interface IUsuarioRepository
{
    Task<PagedResult<Usuario>> ObterTodosUsuarios(int pageNumber, int pageSize, string nome, string email, int? perfilId, DateTime? dataCriacao, bool? statusAcesso);
    Task<Usuario> ObterUsuarioPorId(int id);
    Task<Usuario> ObterUsuarioPorEmail(string email);
    Task AdicionarUsuario(Usuario usuario);
    Task AtualizarUsuario(Usuario usuario);
    Task DeletarUsuario(int id);
    bool Save();
}