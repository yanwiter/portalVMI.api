using Microsoft.AspNetCore.Mvc;
using Vmi.Portal.Common;
using Vmi.Portal.Entities;
using Vmi.Portal.Services.Interfaces;

namespace Vmi.Portal.Controllers;

[ApiController]
[Route("[controller]")]
public class ClientesFornecedoresController : ControllerBase
{
    private readonly IClienteFornecedorService _clienteFornecedorService;
    private readonly ILogger<ClientesFornecedoresController> _logger;

    public ClientesFornecedoresController(
        IClienteFornecedorService clienteFornecedorService,
        ILogger<ClientesFornecedoresController> logger)
    {
        _clienteFornecedorService = clienteFornecedorService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<ClienteFornecedor>>> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string razaoSocial = null,
        [FromQuery] string nomeFantasia = null,
        [FromQuery] string cpfCnpj = null,
        [FromQuery] int? tipoCadastro = null,
        [FromQuery] int? tipoPessoa = null,
        [FromQuery] bool? statusCadastro = null)
    {
        try
        {
            var clientesFornecedores = await _clienteFornecedorService.BuscarTodos(
                pageNumber,
                pageSize,
                razaoSocial,
                nomeFantasia,
                cpfCnpj,
                tipoCadastro,
                tipoPessoa,
                statusCadastro);

            return Ok(clientesFornecedores);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Ocorreu um erro ao processar sua requisição");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ClienteFornecedor>> GetById(Guid id)
    {
        try
        {
            var clienteFornecedor = await _clienteFornecedorService.BuscarPorId(id);
            if (clienteFornecedor == null) return NotFound();
            return Ok(clienteFornecedor);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Ocorreu um erro ao processar sua requisição");
        }
    }

    [HttpPost]
    public async Task<ActionResult<ClienteFornecedor>> Create([FromBody] ClienteFornecedor clienteFornecedor)
    {
        try
        {
            var result = await _clienteFornecedorService.Adicionar(clienteFornecedor);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Ocorreu um erro ao processar sua requisição");
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ClienteFornecedor>> Update(Guid id, [FromBody] ClienteFornecedor clienteFornecedor)
    {
        try
        {
            if (id != clienteFornecedor.Id) return BadRequest();

            var result = await _clienteFornecedorService.Atualizar(clienteFornecedor);
            if (result == null) return NotFound();

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Ocorreu um erro ao processar sua requisição");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var itens = await _clienteFornecedorService.BuscarPorId(id);
            if (itens == null)
            {
                return NotFound();
            }

            await _clienteFornecedorService.Remover(id);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Ocorreu um erro ao processar sua requisição");
        }
    }


    [HttpGet("exportar-excel")]
    public async Task<IActionResult> ExportarParaExcel(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string razaoSocial = null,
        [FromQuery] string nomeFantasia = null,
        [FromQuery] string cpfCnpj = null,
        [FromQuery] int? tipoCadastro = null,
        [FromQuery] int? tipoPessoa = null,
        [FromQuery] bool? statusCadastro = null)
    {
        try
        {
            var excelBytes = await _clienteFornecedorService.ExportarClientesFornecedoresParaExcel(
                pageNumber,
                pageSize,
                razaoSocial,
                nomeFantasia,
                cpfCnpj,
                tipoCadastro,
                tipoPessoa,
                statusCadastro
            );

            return File(excelBytes,
                       "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                       $"clientesfornecedores-{DateTime.Now:yyyyMMddHHmmss}.xlsx");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao exportar Clientes e Fornecedores para Excel");
            return StatusCode(500, "Ocorreu um erro ao exportar os Clientes e Fornecedores para Excel");
        }
    }
}

