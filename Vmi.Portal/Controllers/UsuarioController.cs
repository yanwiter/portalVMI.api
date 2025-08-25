using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.Json;
using System.Threading.Tasks;
using Vmi.Portal.Entities;
using Vmi.Portal.Models;
using Vmi.Portal.Services;
using Vmi.Portal.Utils;
using Vmi.Portal.Enums;

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
        [FromQuery] Guid? idPerfil = null,
        [FromQuery(Name = "dataCriacao")] string dataCriacaoStr = null,
        [FromQuery] bool? statusAcesso = null)
    {
        if (!Util.ConverterDataParaDDMMYYYY(dataCriacaoStr, "dd/MM/yyyy", out DateTime? dataCriacao))
        {
            return BadRequest("Formato de data inválido. Use dd/MM/yyyy.");
        }

        var usuarios = await _usuarioService.ObterTodosUsuarios(
            pageNumber, pageSize, nome, email, idPerfil, dataCriacao, statusAcesso);

        return Ok(usuarios);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Usuario>> GetById(Guid id)
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
    public async Task<ActionResult<Usuario>> GetByEmail(string email)
    {
        try
        {
            var usuario = await _usuarioService.ObterUsuarioPorEmail(email);

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
            usuario.StatusUsuario = StatusUsuarioEnum.Ativo;
            usuario.IsPrimeiroAcesso ??= true;
            usuario.IdRespInclusao = usuario.IdRespInclusao;
            usuario.NomeRespInclusao = criador.Nome;
            usuario.TipoSuspensao = null;
            usuario.DataInicioSuspensao = null;
            usuario.DataFimSuspensao = null;
            usuario.MotivoSuspensao = null;
            usuario.IdRespSuspensao = null;
            usuario.NomeRespSuspensao = null;
            usuario.DataSuspensao = null;

            if (usuario.HorariosAcesso != null)
            {
                var horarios = JsonSerializer.Deserialize<List<HorarioAcesso>>(usuario.HorariosAcesso);
                if (horarios != null)
                {
                    foreach (var horario in horarios)
                    {
                        if (horario.DiaSemana < 0 || horario.DiaSemana > 6)
                            throw new ArgumentException("Dia da semana inválido");
                    }
                }
            }

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
    public async Task<IActionResult> Update(Guid id, [FromBody] Usuario usuarioAtualizado)
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
                if (usuarioComEmail != null && usuarioComEmail.Id.ToString() != id.ToString())
                {
                    return Conflict("Já existe outro usuário cadastrado com este email.");
                }
            }

            var respModificacao = await _usuarioService.ObterUsuarioPorId(usuarioAtualizado.IdRespUltimaAlteracao);
            if (respModificacao == null)
            {
                return BadRequest("O usuário que está modificando não foi encontrado.");
            }

            if (usuarioAtualizado.StatusUsuario != StatusUsuarioEnum.Ativo)
            {
                usuarioAtualizado.IdRespInativacao = respModificacao.Id;
                usuarioAtualizado.NomeRespInativacao = respModificacao.Nome;
                usuarioAtualizado.DataInativacao = Util.PegaHoraBrasilia();
            }

            if (usuarioAtualizado.StatusUsuario != StatusUsuarioEnum.Ativo && usuarioAtualizado.TipoSuspensao.HasValue)
            {
                usuarioAtualizado.IdRespSuspensao = respModificacao.Id;
                usuarioAtualizado.NomeRespSuspensao = respModificacao.Nome;
                usuarioAtualizado.DataSuspensao = Util.PegaHoraBrasilia();
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
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var usuario = await _usuarioService.ObterUsuarioPorId(id);

            if (usuario == null)
            {
                return Ok("Usuário nao encontrado!");
            }

            await _usuarioService.RemoverUsuario(id);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Erro ao remover usuário com ID {id}");
            return StatusCode(500, "Ocorreu um erro ao remover o usuário");
        }
    }

    [HttpPost("{id}/foto-perfil")]
    public async Task<IActionResult> UploadFotoPerfil(Guid id, IFormFile fotoPerfil)
    {
        try
        {
            _logger.LogInformation($"Iniciando upload de foto para usuário {id}");
            
            if (fotoPerfil == null || fotoPerfil.Length == 0)
            {
                _logger.LogWarning($"Nenhuma imagem foi enviada para usuário {id}");
                return BadRequest("Nenhuma imagem foi enviada");
            }

            if (fotoPerfil.Length > 5 * 1024 * 1024)
            {
                _logger.LogWarning($"Imagem muito grande para usuário {id}: {fotoPerfil.Length} bytes");
                return BadRequest("A imagem deve ter no máximo 5MB");
            }

            if (!fotoPerfil.ContentType.StartsWith("image/"))
            {
                _logger.LogWarning($"Tipo de arquivo inválido para usuário {id}: {fotoPerfil.ContentType}");
                return BadRequest("O arquivo deve ser uma imagem");
            }

            _logger.LogInformation($"Validando usuário {id}");
            var usuario = await _usuarioService.ObterUsuarioPorId(id);
            if (usuario == null)
            {
                _logger.LogWarning($"Usuário {id} não encontrado");
                return NotFound("Usuário não encontrado");
            }

            _logger.LogInformation($"Processando imagem para usuário {id}");
  
            using var memoryStream = new MemoryStream();
            await fotoPerfil.CopyToAsync(memoryStream);
            var imageBytes = memoryStream.ToArray();
            var base64String = Convert.ToBase64String(imageBytes);
            var dataUrl = $"data:{fotoPerfil.ContentType};base64,{base64String}";

            _logger.LogInformation($"Atualizando usuário {id} com nova foto");

            await _usuarioService.AtualizarFotoPerfil(id, dataUrl);

            _logger.LogInformation($"Foto de perfil atualizada com sucesso para usuário {id}");
            return Ok(new { message = "Foto de perfil atualizada com sucesso", fotoPerfil = dataUrl });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Erro ao fazer upload da foto de perfil para usuário com ID {id}");
            return StatusCode(500, "Ocorreu um erro ao processar a imagem");
        }
    }

    [HttpDelete("{id}/foto-perfil")]
    public async Task<IActionResult> RemoverFotoPerfil(Guid id)
    {
        try
        {
            _logger.LogInformation($"Iniciando remoção de foto para usuário {id}");
            
            var usuario = await _usuarioService.ObterUsuarioPorId(id);
            if (usuario == null)
            {
                _logger.LogWarning($"Usuário {id} não encontrado para remoção de foto");
                return NotFound("Usuário não encontrado");
            }

            _logger.LogInformation($"Removendo foto de perfil do usuário {id}");

            await _usuarioService.RemoverFotoPerfil(id);

            _logger.LogInformation($"Foto de perfil removida com sucesso do usuário {id}");
            return Ok(new { message = "Foto de perfil removida com sucesso" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Erro ao remover foto de perfil do usuário com ID {id}");
            return StatusCode(500, "Ocorreu um erro ao remover a foto de perfil");
        }
    }

    [HttpGet("exportar-excel")]
    public async Task<IActionResult> UsuariosReportExport(
    [FromQuery] string nome = null,
    [FromQuery] string email = null,
    [FromQuery] Guid? perfilId = null,
    [FromQuery(Name = "dataCriacao")] string dataCriacaoStr = null,
    [FromQuery] bool? statusAcesso = null)
    {
        try
        {
            if (!Util.ConverterDataParaDDMMYYYY(dataCriacaoStr, "dd/MM/yyyy", out DateTime? dataCriacao))
            {
                return BadRequest("Formato de data inválido. Use dd/MM/yyyy.");
            }

            var excelBytes = await _usuarioService.ExportarUsuariosParaExcel(1,10,
                nome, email, perfilId, dataCriacao, statusAcesso);

            return File(excelBytes,
                      "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                      $"usuarios-{DateTime.Now:yyyyMMddHHmmss}.xlsx");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao exportar usuários para Excel");
            return StatusCode(500, "Ocorreu um erro ao exportar os usuários para Excel");
        }
    }
}