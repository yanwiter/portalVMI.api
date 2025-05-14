using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Threading.Tasks;
using Vmi.Portal.Entities;
using Vmi.Portal.Models;
using Vmi.Portal.Services;
using Vmi.Portal.Utils;

namespace Vmi.Portal.Controllers;

[ApiController]
[Route("[controller]")]
public class UsuarioController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;
    private readonly ILogger<UsuarioController> _logger;

    public UsuarioController(
        IUsuarioService usuarioService,
        ILogger<UsuarioController> logger)
    {
        _usuarioService = usuarioService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Usuario>>> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string nome = null,
        [FromQuery] string email = null,
        [FromQuery] int? perfilId = null,
        [FromQuery(Name = "dataCriacao")] string dataCriacaoStr = null,
        [FromQuery] bool? statusAcesso = null)
    {
        if (!Util.ConverterDataParaDDMMYYYY(dataCriacaoStr, "dd/MM/yyyy", out DateTime? dataCriacao))
        {
            return BadRequest("Formato de data inválido. Use dd/MM/yyyy.");
        }

        var usuarios = await _usuarioService.ObterTodosUsuarios(
            pageNumber, pageSize, nome, email, perfilId, dataCriacao, statusAcesso);

        return Ok(usuarios);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Usuario>> GetById(int id)
    {
        try
        {
            var usuario = await _usuarioService.ObterUsuarioPorId(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return Ok(usuario);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Erro ao obter usuário com ID {id}");
            return StatusCode(500, "Ocorreu um erro ao processar sua requisição");
        }
    }


    [HttpGet("por-email/{email}")]
    public ActionResult<Usuario> GetByEmail(string email)
    {
        try
        {
            var usuario = _usuarioService.ObterUsuarioPorEmail(email);

            if (usuario == null)
            {
                return NotFound();
            }

            return Ok(usuario);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Erro ao obter usuário com email {email}");
            return StatusCode(500, "Ocorreu um erro ao processar sua requisição");
        }
    }

    [HttpPost]
    public async Task<ActionResult<Usuario>> Create([FromBody] Usuario usuario)
    {
        try
        {
            if (string.IsNullOrEmpty(usuario.Nome))
            {
                return BadRequest("O nome do usuário é obrigatório.");
            }

            if (string.IsNullOrEmpty(usuario.Email))
            {
                return BadRequest("O email do usuário é obrigatório.");
            }

            var usuarioExistente = await _usuarioService.ObterUsuarioPorEmail(usuario.Email);
            if (usuarioExistente != null)
            {
                return Conflict("Já existe um usuário cadastrado com este email.");
            }

            var criador = await _usuarioService.ObterUsuarioPorId(usuario.IdRespInclusao);
            if (criador == null)
            {
                return BadRequest("O usuário criador não foi encontrado.");
            }

            usuario.DataInclusao = Util.PegaHoraBrasilia();
            usuario.StatusUsuario = true;
            usuario.IsPrimeiroAcesso ??= true;
            usuario.IdRespInclusao = usuario.IdRespInclusao;
            usuario.NomeRespInclusao = criador.Nome;

            await _usuarioService.AdicionarUsuario(usuario);
            await _usuarioService.GerarEmailBoasVindas(usuario.Email);

            return CreatedAtAction(nameof(GetById), new { id = usuario.Id }, usuario);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar usuário");
            return StatusCode(500, "Ocorreu um erro ao criar o usuário");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Usuario usuarioAtualizado)
    {
        try
        {
            if (id != usuarioAtualizado.Id)
            {
                return BadRequest("ID do usuário não corresponde ao ID na URL.");
            }

            var usuarioExistente = await _usuarioService.ObterUsuarioPorId(id);
            if (usuarioExistente == null)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(usuarioAtualizado.Nome))
            {
                return BadRequest("O nome do usuário é obrigatório.");
            }

            if (string.IsNullOrEmpty(usuarioAtualizado.Email))
            {
                return BadRequest("O email do usuário é obrigatório.");
            }

            if (usuarioExistente.Email != usuarioAtualizado.Email)
            {
                var usuarioComEmail = _usuarioService.ObterUsuarioPorEmail(usuarioAtualizado.Email);
                if (usuarioComEmail != null && usuarioComEmail.Id != id)
                {
                    return Conflict("Já existe outro usuário cadastrado com este email.");
                }
            }

            var respModificacao = await _usuarioService.ObterUsuarioPorId(usuarioAtualizado.IdRespUltimaAlteracao);
            if (respModificacao == null)
            {
                return BadRequest("O usuário que está modificando não foi encontrado.");
            }

            if (!usuarioAtualizado.StatusUsuario)
            {
                usuarioAtualizado.IdRespInativacao = respModificacao.Id;
                usuarioAtualizado.NomeRespInativacao = respModificacao.Nome;
                usuarioAtualizado.DataInativacao = Util.PegaHoraBrasilia();
            }

            usuarioAtualizado.IdRespUltimaAlteracao = respModificacao.Id;
            usuarioAtualizado.NomeRespUltimaAlteracao = respModificacao.Nome;
            usuarioAtualizado.DataUltimaAlteracao = Util.PegaHoraBrasilia();

            await _usuarioService.AtualizarUsuario(usuarioAtualizado);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Erro ao atualizar usuário com ID {id}");
            return StatusCode(500, "Ocorreu um erro ao atualizar o usuário");
        }
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        try
        {
            var usuario = _usuarioService.ObterUsuarioPorId(id);
            if (usuario == null)
            {
                return NotFound();
            }

            _usuarioService.RemoverUsuario(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Erro ao remover usuário com ID {id}");
            return StatusCode(500, "Ocorreu um erro ao remover o usuário");
        }
    }
}