using ClosedXML.Excel;
using System.Reflection;
using Vmi.Portal.Common;
using Vmi.Portal.Entities;
using Vmi.Portal.Models;
using Vmi.Portal.Repositories;
using Vmi.Portal.Repositories.Interfaces;
using Vmi.Portal.Enums;

namespace Vmi.Portal.Services;

public class PerfilService(
    IPerfilRepository _perfilRepository,
    IPerfilRotinaRepository _perfilRotinaRepository,
    IRotinaRepository _rotinaRepository) : IPerfilService
{
    public async Task<PagedResult<Perfil>> ObterTodosPerfis(int pageNumber, int pageSize, string nome, StatusPerfilEnum? statusPerfil)
    {
        return await _perfilRepository.ObterTodosPerfis(pageNumber, pageSize, nome, statusPerfil);
    }

    public async Task<InformacaoPerfil> ObterTodosAcessoPerfis()
    {
        //verificar paginação nesse caso
        var perfilTask = _perfilRepository.ObterTodosPerfis(1, 100, null, null);
        var rotinasTask = _rotinaRepository.ObterRotinas("", "");

        await Task.WhenAll(perfilTask, rotinasTask);
        var perfil = perfilTask.Result;
        if (perfil == null) return null;

        var rotinas = rotinasTask.Result;

        Guid[] acessosEnums =
        [
            AcessoConst.Visualizacao,
            AcessoConst.Inclusao,
            AcessoConst.Edicao,
            AcessoConst.Exclusao
        ];

        HashSet<AcessoPerfil> acessosModel = new(
            (from a in acessosEnums
             from r in rotinas
             orderby r.ModuloNome, r.Nome
             select new AcessoPerfil
             {
                 IdAcesso = a,
                 Acesso = AcessoConst.ObterNomePorGuid(a),
                 IdRotina = r.Id,
                 Rotina = r.Nome,
                 IdModulo = r.IdModulo,
                 Modulo = r.ModuloNome,
             }));

        return new InformacaoPerfil
        {
            Acessos = acessosModel,
            Perfil = new()
        };
    }

    public async Task<Perfil> ObterPerfilPorId(Guid id)
    {
        return await _perfilRepository.ObterPerfilPorId(id);
    }

    public async Task<Result<InformacaoPerfil>> ObterInformacoesDoPerfil(Guid id, Guid acesso, string rotina, string modulo)
    {
        var perfilTask = _perfilRepository.ObterPerfilPorId(id);
        var rotinasTask = _rotinaRepository.ObterRotinas(rotina, modulo);
        var perfilRotinasTask = _perfilRotinaRepository.ObterPerfilRotinaPorIdPerfil(id);

        await Task.WhenAll(perfilTask, rotinasTask, perfilRotinasTask);

        var perfil = perfilTask.Result;
        if (perfil == null) return null;

        var rotinas = rotinasTask.Result;
        var perfilRotinas = perfilRotinasTask.Result;

        var perfilRotinasSet = perfilRotinas
            .Select(p => (p.IdAcesso, p.IdRotina))
            .ToHashSet();

        Guid[] acessosEnums =
        [
            AcessoConst.Visualizacao,
            AcessoConst.Inclusao,
            AcessoConst.Edicao,
            AcessoConst.Exclusao
        ];

        if (acesso != AcessoConst.None)
        {
            acessosEnums = [acesso];
        }

        HashSet<AcessoPerfil> acessosModel = new(
            (from a in acessosEnums
             from r in rotinas
             orderby r.ModuloNome, r.Nome
             select new AcessoPerfil
             {
                 IdAcesso = a,
                 Acesso = AcessoConst.ObterNomePorGuid(a),
                 IdRotina = r.Id,
                 Rotina = r.Nome,
                 IdModulo = r.IdModulo,
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
        informacaoPerfil.Perfil.Id = await _perfilRepository.AdicionarPerfil(informacaoPerfil.Perfil);

        await _perfilRotinaRepository.InserirPerfilRotinas(informacaoPerfil.Acessos
            .Where(c => c.Ativo)
            .Select(item => new PerfilRotina
            {
                IdPerfil = informacaoPerfil.Perfil.Id,
                IdRotina = item.IdRotina,
                IdAcesso = item.IdAcesso
            }));

        return informacaoPerfil.Perfil;
    }

    public async Task AtualizarPerfil(InformacaoPerfil informacaoPerfil)
    {
        await _perfilRepository.AtualizarPerfil(informacaoPerfil.Perfil);
        await _perfilRotinaRepository.ExcluirPerfilRotina(informacaoPerfil.Perfil.Id);
        
        await _perfilRotinaRepository.InserirPerfilRotinas(informacaoPerfil.Acessos
            .Where(c => c.Ativo)
            .Select(item => new PerfilRotina
            {
                IdPerfil = informacaoPerfil.Perfil.Id,
                IdRotina = item.IdRotina,
                IdAcesso = item.IdAcesso
            }));
    }

    public async Task<byte[]> ExportarPerfisParaExcel(int pageNumber, int pageSize, string nome = null, StatusPerfilEnum? statusPerfil = null)
    {
        var perfis = await _perfilRepository.ObterTodosPerfis(pageNumber, pageSize, nome, statusPerfil);

        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Perfis");

            string[] headers = {
                "ID",
                "Nome",
                "Data de Inclusão",
                "Resp. Inclusão",
                "Status",
                "Data Últ. Modificação",
                "Resp Últ. Modificação",
                "Justificativa Inativação",
                "Tipo Suspensão",
                "Data Início Suspensão",
                "Data Fim Suspensão",
                "Motivo Suspensão",
                "Resp. Suspensão",
                "Data Suspensão"
            };

            for (int col = 1; col <= headers.Length; col++)
            {
                worksheet.Cell(1, col).Value = headers[col - 1];
                worksheet.Cell(1, col).Style.Font.Bold = true;
            }

            int row = 2;
            foreach (var perfil in perfis.Items)
            {
                worksheet.Cell(row, 1).Value = perfil.Id.ToString();
                worksheet.Cell(row, 2).Value = perfil.Nome;
                worksheet.Cell(row, 3).Value = perfil.DataInclusao?.ToString("dd/MM/yyyy 'às' HH:mm:ss") ?? "N/A";
                worksheet.Cell(row, 4).Value = perfil.NomeRespInclusao;
                worksheet.Cell(row, 5).Value = ObterStatusFormatado(perfil.StatusPerfil);
                worksheet.Cell(row, 6).Value = perfil.DataUltimaModificacao?.ToString("dd/MM/yyyy 'às' HH:mm:ss") ?? "N/A";
                worksheet.Cell(row, 7).Value = perfil.NomeRespUltimaModificacao ?? "N/A";
                worksheet.Cell(row, 8).Value = perfil.JustificativaInativacao ?? "-";
                worksheet.Cell(row, 9).Value = ObterTipoSuspensaoFormatado(perfil.TipoSuspensao);
                worksheet.Cell(row, 10).Value = perfil.DataInicioSuspensao?.ToString("dd/MM/yyyy") ?? "N/A";
                worksheet.Cell(row, 11).Value = perfil.DataFimSuspensao?.ToString("dd/MM/yyyy") ?? "N/A";
                worksheet.Cell(row, 12).Value = perfil.MotivoSuspensao ?? "-";
                worksheet.Cell(row, 13).Value = perfil.NomeRespSuspensao ?? "-";
                worksheet.Cell(row, 14).Value = perfil.DataSuspensao?.ToString("dd/MM/yyyy 'às' HH:mm:ss") ?? "N/A";
                row++;
            }

            worksheet.Columns().AdjustToContents();

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                return stream.ToArray();
            }
        }
    }

    private string ObterStatusFormatado(StatusPerfilEnum status)
    {
        return status switch
        {
            StatusPerfilEnum.Ativo => "Ativo",
            StatusPerfilEnum.Inativo => "Inativo",
            StatusPerfilEnum.Suspenso => "Suspenso",
            _ => "Desconhecido"
        };
    }

    private string ObterTipoSuspensaoFormatado(TipoSuspensaoEnum? tipo)
    {
        if (!tipo.HasValue)
            return "N/A";
            
        return tipo.Value switch
        {
            TipoSuspensaoEnum.Temporaria => "Temporária",
            TipoSuspensaoEnum.Permanente => "Permanente",
            _ => "N/A"
        };
    }

    public async Task AdicionarPerfil(Perfil perfil)
    {
        await _perfilRepository.AdicionarPerfil(perfil);
    }

    public async Task AtualizarPerfil(Perfil perfil, Guid usuarioId)
    {
        await _perfilRepository.AtualizarPerfil(perfil);
    }

    public async Task RemoverPerfil(Guid id)
    {
        await _perfilRepository.RemoverPerfil(id);
    }

    public async Task VerificarSuspensoesExpiradas()
    {
        try
        {
            var perfisSuspensos = await _perfilRepository.ObterPerfisSuspensosTemporarios();
            var perfisParaReativar = new List<Perfil>();

            foreach (var perfil in perfisSuspensos)
            {
                if (perfil.DataFimSuspensao.HasValue && perfil.DataFimSuspensao.Value <= DateTime.Now)
                {
                    perfil.StatusPerfil = StatusPerfilEnum.Ativo;
                    perfil.DataUltimaModificacao = DateTime.Now;
                    perfisParaReativar.Add(perfil);
                }
            }

            foreach (var perfil in perfisParaReativar)
            {
                await _perfilRepository.AtualizarPerfil(perfil);
            }
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<List<Perfil>> ObterPerfisSuspensos()
    {
        try
        {
            return await _perfilRepository.ObterPerfisSuspensos();
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<List<Perfil>> ObterPerfisSuspensosTemporarios()
    {
        try
        {
            return await _perfilRepository.ObterPerfisSuspensosTemporarios();
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<List<Perfil>> ObterPerfisSuspensosPermanentes()
    {
        try
        {
            return await _perfilRepository.ObterPerfisSuspensosPermanentes();
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<bool> VerificarSePerfilEstaSuspenso(Guid perfilId)
    {
        try
        {
            var perfil = await _perfilRepository.ObterPerfilPorId(perfilId);
            if (perfil == null) return false;

            if (perfil.StatusPerfil != StatusPerfilEnum.Suspenso)
                return false;

            if (perfil.TipoSuspensao == TipoSuspensaoEnum.Temporaria && 
                perfil.DataFimSuspensao.HasValue && 
                perfil.DataFimSuspensao.Value <= DateTime.Now)
            {
                perfil.StatusPerfil = StatusPerfilEnum.Ativo;
                perfil.DataUltimaModificacao = DateTime.Now;
                await _perfilRepository.AtualizarPerfil(perfil);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}