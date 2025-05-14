using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Vmi.Portal.Common;
using Vmi.Portal.Entities;

namespace Vmi.Portal.Services;

public interface IUsuarioService
{
    Task<PagedResult<Usuario>> ObterTodosUsuarios(int pageNumber, int pageSize, string nome, string email, int? perfilId, DateTime? dataCriacao = null, bool? statusAcesso = null);
    Task<Usuario> ObterUsuarioPorId(int id);
    Task<Usuario> ObterUsuarioPorEmail(string email);
    Task<(bool Success, string Message)> GerarEmailBoasVindas(string email);
    Task AdicionarUsuario(Usuario usuario);
    Task AtualizarUsuario(Usuario usuario);
    Task RemoverUsuario(int id);
    bool Salvar();
}