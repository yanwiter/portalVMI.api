using Microsoft.AspNetCore.Mvc;
using Vmi.Portal.Entities;
using Vmi.Portal.Enums;
using Vmi.Portal.Models;
using Vmi.Portal.Services;
using Vmi.Portal.Utils;

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
        [FromQuery] bool? statusPerfil = null)
    {
        try
        {
            var perfis = await _perfilService.ObterTodosPerfis(pageNumber, pageSize, nome, statusPerfil);
            return Ok(perfis);
        }
        catch (Exception ex)
        {
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
            _logger.LogError(ex, "Erro ao obter perfis");
            return StatusCode(500, "Ocorreu um erro ao processar sua requisição");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetById(
        int id,
        AcessoEnum acesso,
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
    public async Task<IActionResult> Update(int id, [FromBody] InformacaoPerfil informacaoPerfil, int usuarioId)
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

            if (!informacaoPerfil.Perfil.StatusPerfil)
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
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var perfil = await _perfilService.ObterPerfilPorId(id);
            if (perfil == null)
            {
                return NotFound();
            }

            await _perfilService.RemoverPerfil(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Erro ao remover perfil com ID {id}");
            return StatusCode(500, "Ocorreu um erro ao remover o perfil");
        }
    }
}