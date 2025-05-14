using System.Reflection;
using Vmi.Portal.Common;
using Vmi.Portal.Entities;
using Vmi.Portal.Enums;
using Vmi.Portal.Models;
using Vmi.Portal.Repositories.Interfaces;

namespace Vmi.Portal.Services;

public class PerfilService(
    IPerfilRepository perfilRepository,
    IPerfilRotinaRepository perfilRotinaRepository,
    IRotinaRepository rotinaRepository) : IPerfilService
{
    public async Task<PagedResult<Perfil>> ObterTodosPerfis(int pageNumber, int pageSize, string nome, bool? statusPerfil)
    {
        return await perfilRepository.ObterTodosPerfis(pageNumber, pageSize, nome, statusPerfil);
    }

    public async Task<InformacaoPerfil> ObterTodosAcessoPerfis()
    {
        //verificar paginação nesse caso
        var perfilTask = perfilRepository.ObterTodosPerfis(1, 100, null, null);
        var rotinasTask = rotinaRepository.ObterRotinas("", "");

        await Task.WhenAll(perfilTask, rotinasTask);
        var perfil = perfilTask.Result;
        if (perfil == null) return null;

        var rotinas = rotinasTask.Result;

        AcessoEnum[] acessosEnums =
        [
            AcessoEnum.Visualizacao,
            AcessoEnum.Inclusao,
            AcessoEnum.Edicao,
            AcessoEnum.Exclusao
        ];

        HashSet<AcessoPerfil> acessosModel = new(
            (from a in acessosEnums
             from r in rotinas
             orderby r.ModuloNome, r.Nome
             select new AcessoPerfil
             {
                 AcessoId = a,
                 Acesso = a.GetDescription(),
                 RotinaId = r.Id,
                 Rotina = r.Nome,
                 ModuloId = r.ModuloId,
                 Modulo = r.ModuloNome,
             }));

        return new InformacaoPerfil
        {
            Acessos = acessosModel,
            Perfil = new()
        };
    }

    public async Task<Perfil> ObterPerfilPorId(int id)
    {
        return await perfilRepository.ObterPerfilPorId(id);
    }

    public async Task<Result<InformacaoPerfil>> ObterInformacoesDoPerfil(
        int id,
        AcessoEnum acesso,
        string rotina,
        string modulo)
    {
        var perfilTask = perfilRepository.ObterPerfilPorId(id);
        var rotinasTask = rotinaRepository.ObterRotinas(rotina, modulo);
        var perfilRotinasTask = perfilRotinaRepository.ObterPerfilRotinaPorIdPerfil(id);

        await Task.WhenAll(perfilTask, rotinasTask, perfilRotinasTask);

        var perfil = perfilTask.Result;
        if (perfil == null) return null;

        var rotinas = rotinasTask.Result;
        var perfilRotinas = perfilRotinasTask.Result;

        var perfilRotinasSet = perfilRotinas
            .Select(p => (p.AcessoId, p.RotinaId))
            .ToHashSet();

        AcessoEnum[] acessosEnums =
        [
            AcessoEnum.Visualizacao,
            AcessoEnum.Inclusao,
            AcessoEnum.Edicao,
            AcessoEnum.Exclusao
        ];

        if (acesso != AcessoEnum.None)
        {
            acessosEnums = [acesso];
        }

        HashSet<AcessoPerfil> acessosModel = new(
            (from a in acessosEnums
             from r in rotinas
             orderby r.ModuloNome, r.Nome
             select new AcessoPerfil
             {
                 AcessoId = a,
                 Acesso = a.GetDescription(),
                 RotinaId = r.Id,
                 Rotina = r.Nome,
                 ModuloId = r.ModuloId,
                 Modulo = r.ModuloNome,
                 Ativo = perfilRotinasSet.Contains((a, r.Id))
             }));

        return new()
        {
            IsSuccess = true,
            Data = new InformacaoPerfil
            {
                Perfil = perfil,
                Acessos = acessosModel
            }
        };
    }

    public async Task<Perfil> SalvarPerfil(InformacaoPerfil informacaoPerfil)
    {
        informacaoPerfil.Perfil.Id = await perfilRepository.AdicionarPerfil(informacaoPerfil.Perfil);

        await perfilRotinaRepository.InserirPerfilRotinas(informacaoPerfil.Acessos
            .Where(c => c.Ativo)
            .Select(item => new PerfilRotina
            {
                PerfilId = informacaoPerfil.Perfil.Id,
                RotinaId = item.RotinaId,
                AcessoId = item.AcessoId
            }));

        return informacaoPerfil.Perfil;
    }

    public async Task AtualizarPerfil(InformacaoPerfil informacaoPerfil)
    {
        await perfilRepository.AtualizarPerfil(informacaoPerfil.Perfil);
        await perfilRotinaRepository.ExcluirPerfilRotina(informacaoPerfil.Perfil.Id);
        await perfilRotinaRepository.InserirPerfilRotinas(informacaoPerfil.Acessos
            .Where(c => c.Ativo)
            .Select(item => new PerfilRotina
            {
                PerfilId = informacaoPerfil.Perfil.Id,
                RotinaId = item.RotinaId,
                AcessoId = item.AcessoId
            }));
    }

    public async Task AdicionarPerfil(Perfil perfil)
    {
        await perfilRepository.AdicionarPerfil(perfil);
    }

    public async Task AtualizarPerfil(Perfil perfil, int usuarioId)
    {
        await perfilRepository.AtualizarPerfil(perfil);
    }

    public async Task RemoverPerfil(int id)
    {
        await perfilRepository.RemoverPerfil(id);
    }
}