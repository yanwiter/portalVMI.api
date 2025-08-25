using Microsoft.AspNetCore.Mvc;
using Vmi.Portal.Entities;
using Vmi.Portal.Models;
using Vmi.Portal.Services;
using Vmi.Portal.Utils;
using Vmi.Portal.Enums;

namespace Vmi.Portal.Controllers;

[ApiController]
[Route("[controller]")]
public class PerfilController : ControllerBase
{
    private readonly IPerfilService _perfilService;
    private readonly IUsuarioService _usuarioService;
    private readonly ILogger<PerfilController> _logger;

    public PerfilController(
        IPerfilService perfilService,
        IUsuarioService usuarioService,
        ILogger<PerfilController> logger)
    {
        _perfilService = perfilService;
        _usuarioService = usuarioService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Perfil>>> ObterTodosPerfis(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string nome = null,
        [FromQuery] StatusPerfilEnum? statusPerfil = null)
    {
        try
        {
            var perfis = await _perfilService.ObterTodosPerfis(pageNumber, pageSize, nome, statusPerfil);
            return Ok(perfis);
        }
        catch (Exception ex)
        {
            throw;
            _logger.LogError(ex, "Erro ao obter perfis");
            return StatusCode(500, "Ocorreu um erro ao processar sua requisição");
        }
    }

    [HttpGet("Acessos")]
    public async Task<ActionResult<IEnumerable<Perfil>>> GetAll()
    {
        try
        {
            var perfis = await _perfilService.ObterTodosAcessoPerfis();
            return Ok(perfis);
        }
        catch (Exception ex)
        {
            throw;
            _logger.LogError(ex, "Erro ao obter perfis");
            return StatusCode(500, "Ocorreu um erro ao processar sua requisição");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetById(
        Guid id,
        Guid acesso,
        string rotina,
        string modulo)
    {
        try
        {
            return Ok(await _perfilService.ObterInformacoesDoPerfil(id, acesso, rotina, modulo));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao Obter Informações do Perfil");
            return StatusCode(500, "Ocorreu um erro ao processar sua requisição");
        }
    }

    [HttpPost]
    public async Task<ActionResult<Perfil>> Create([FromBody] InformacaoPerfil informacaoPerfil)
    {
        try
        {
            if (string.IsNullOrEmpty(informacaoPerfil.Perfil.Nome))
            {
                return BadRequest("O nome do perfil é obrigatório.");
            }

            var respInclusao = await _usuarioService.ObterUsuarioPorId(informacaoPerfil.Perfil.IdRespInclusao);
            if (respInclusao == null)
            {
                return BadRequest("O usuário reponsável pela inclusão não foi encontrado.");
            }

            informacaoPerfil.Perfil.DataInclusao = Util.PegaHoraBrasilia();
            informacaoPerfil.Perfil.NomeRespInclusao = respInclusao.Nome;

             return Ok(await _perfilService.SalvarPerfil(informacaoPerfil));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar perfil");
            return StatusCode(500, "Ocorreu um erro ao criar o perfil");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] InformacaoPerfil informacaoPerfil, int usuarioId)
    {
        try
        {
            var perfilExistente = await _perfilService.ObterPerfilPorId(id);
            if (perfilExistente == null)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(informacaoPerfil.Perfil.Nome))
            {
                return BadRequest("O nome do perfil é obrigatório.");
            }

            informacaoPerfil.Perfil.Id = id;

            var respModificacao = await _usuarioService.ObterUsuarioPorId(informacaoPerfil.Perfil.IdRespUltimaModificacao);
            if (respModificacao == null)
            {
                return BadRequest("O usuário que está modificando não foi encontrado.");
            }

            if (informacaoPerfil.Perfil.StatusPerfil == StatusPerfilEnum.Suspenso)
            {
                if (string.IsNullOrEmpty(informacaoPerfil.Perfil.MotivoSuspensao))
                {
                    return BadRequest("O motivo da suspensão é obrigatório.");
                }

                if (informacaoPerfil.Perfil.DataInicioSuspensao == null)
                {
                    return BadRequest("A data de início da suspensão é obrigatória.");
                }

                if (informacaoPerfil.Perfil.TipoSuspensao == TipoSuspensaoEnum.Temporaria)
                {
                    if (informacaoPerfil.Perfil.DataFimSuspensao == null)
                    {
                        return BadRequest("A data de fim da suspensão é obrigatória para suspensões temporárias.");
                    }

                    if (informacaoPerfil.Perfil.DataFimSuspensao <= informacaoPerfil.Perfil.DataInicioSuspensao)
                    {
                        return BadRequest("A data de fim da suspensão deve ser posterior à data de início.");
                    }
                }

                informacaoPerfil.Perfil.IdRespSuspensao = respModificacao.Id;
                informacaoPerfil.Perfil.NomeRespSuspensao = respModificacao.Nome;
            }

            if (informacaoPerfil.Perfil.StatusPerfil == StatusPerfilEnum.Inativo)
            {
                informacaoPerfil.Perfil.IdRespInativacao = respModificacao.Id;
                informacaoPerfil.Perfil.NomeRespInativacao = respModificacao.Nome;
                informacaoPerfil.Perfil.DataInativacao = Util.PegaHoraBrasilia();
            }

            informacaoPerfil.Perfil.NomeRespUltimaModificacao = respModificacao.Nome;
            informacaoPerfil.Perfil.DataUltimaModificacao = Util.PegaHoraBrasilia();

            await _perfilService.AtualizarPerfil(informacaoPerfil);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Erro ao atualizar perfil com ID {id}");
            return StatusCode(500, "Ocorreu um erro ao atualizar o perfil");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var perfil = await _perfilService.ObterPerfilPorId(id);
            if (perfil == null)
            {
                return NotFound();
            }

            await _perfilService.RemoverPerfil(id);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Erro ao remover perfil com ID {id}");
            return StatusCode(500, "Ocorreu um erro ao remover o perfil");
        }
    }

    [HttpGet("exportar-excel")]
    public async Task<IActionResult> ExportarParaExcel(
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] string nome = null,
    [FromQuery] StatusPerfilEnum? statusPerfil = null)
    {
        try
        {
            var excelBytes = await _perfilService.ExportarPerfisParaExcel(pageNumber, pageSize, nome, statusPerfil);

            return File(excelBytes,
                       "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                       $"perfis-{DateTime.Now:yyyyMMddHHmmss}.xlsx");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao exportar perfis para Excel");
            return StatusCode(500, "Ocorreu um erro ao exportar os perfis para Excel");
        }
    }
}