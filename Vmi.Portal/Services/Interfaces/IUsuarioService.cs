using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Vmi.Portal.Common;
using Vmi.Portal.Entities;

namespace Vmi.Portal.Services;

public interface IUsuarioService
{
    Task<PagedResult<Usuario>> ObterTodosUsuarios(int pageNumber, int pageSize, string nome, string email, Guid? idPerfil, DateTime? dataCriacao = null, bool? statusAcesso = null);
    Task<Usuario> ObterUsuarioPorId(Guid id);
    Task<Usuario> ObterUsuarioPorEmail(string email);
    Task AtualizarFotoPerfil(Guid id, string fotoPerfil);
    Task RemoverFotoPerfil(Guid id);
    Task<(bool Success, string Message)> GerarEmailBoasVindas(string email);
    Task<byte[]> ExportarUsuariosParaExcel(int pageNumber, int pageSize, string nome = null, string email = null, Guid? perfilId = null, DateTime? dataCriacao = null, bool? statusAcesso = null);
    Task AdicionarUsuario(Usuario usuario);
    Task AtualizarUsuario(Usuario usuario);
    Task RemoverUsuario(Guid id);
    bool Salvar();
}