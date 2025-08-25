using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Vmi.Portal.Common;
using Vmi.Portal.DbContext;
using Vmi.Portal.Entities;
using Vmi.Portal.Repositories;
using Vmi.Portal.Repositories.Interfaces;
using Vmi.Portal.Services.Interfaces;
using Vmi.Portal.Utils;
using static Vmi.Portal.Common.Enums;

namespace Vmi.Portal.Services;

public class ClienteFornecedorService : IClienteFornecedorService
{
    private readonly IClienteFornecedorRepository _clienteFornecedorRepository;
    private readonly IEnderecoRepository _enderecoRepository;
    private readonly IContatoRepository _contatoRepository;
    private readonly IDadoFinanceiroRepository _dadoFinanceiroRepository;
    private readonly IAnexoRepository _anexoRepository;

    public ClienteFornecedorService(
        IClienteFornecedorRepository clienteFornecedorRepository,
        IEnderecoRepository enderecoRepository,
        IContatoRepository contatoRepository,
        IDadoFinanceiroRepository dadoFinanceiroRepository,
        IAnexoRepository anexoRepository
    )
    {
        _clienteFornecedorRepository = clienteFornecedorRepository;
        _enderecoRepository = enderecoRepository;
        _contatoRepository = contatoRepository;
        _dadoFinanceiroRepository = dadoFinanceiroRepository;
        _anexoRepository = anexoRepository;
    }

    public async Task<PagedResult<ClienteFornecedor>> BuscarTodos(
        int pageNumber,
        int pageSize,
        string razaoSocial = null,
        string nomeFantasia = null,
        string cpfCnpj = null,
        int? tipoCadastro = null,
        int? tipoPessoa = null,
        bool? status = null)
    {

        var pagedResult = await _clienteFornecedorRepository.BuscarTodos(
            pageNumber, pageSize, razaoSocial, nomeFantasia, cpfCnpj,
            tipoCadastro, tipoPessoa, status);

        var ids = pagedResult.Items.Select(c => c.Id).ToList();

        var enderecosTask = _enderecoRepository.GetByIdClienteFornecedorAsync(ids);
        var contatosTask = _contatoRepository.GetByIdClienteFornecedorAsync(ids);
        var dadosFinanceirosTask = _dadoFinanceiroRepository.GetByIdClienteFornecedorAsync(ids);
       var anexosTask = _anexoRepository.GetByIdClienteFornecedorAsync(ids);

        await Task.WhenAll(enderecosTask, contatosTask, dadosFinanceirosTask, anexosTask);

        var enderecosPorCliente = (await enderecosTask).GroupBy(e => e.IdClienteFornecedor).ToDictionary(g => g.Key, g => g.ToList());
        var contatosPorCliente = (await contatosTask).GroupBy(c => c.IdClienteFornecedor).ToDictionary(g => g.Key, g => g.ToList());
        var dadosPorCliente = (await dadosFinanceirosTask).GroupBy(d => d.IdClienteFornecedor).ToDictionary(g => g.Key, g => g.ToList());
        var anexosPorCliente = (await anexosTask).GroupBy(a => a.IdClienteFornecedor).ToDictionary(g => g.Key, g => g.ToList());

        foreach (var cliente in pagedResult.Items)
        {
            if (enderecosPorCliente.TryGetValue(cliente.Id, out var enderecos))
                cliente.Enderecos = enderecos;

            if (contatosPorCliente.TryGetValue(cliente.Id, out var contatos))
                cliente.Contatos = contatos;

            if (dadosPorCliente.TryGetValue(cliente.Id, out var dadosFinanceiros))
                cliente.DadosFinanceiros = dadosFinanceiros;

            if (anexosPorCliente.TryGetValue(cliente.Id, out var anexos))
                cliente.Anexos = anexos;
        }

        return pagedResult;
    }

    public async Task<ClienteFornecedor> BuscarPorId(Guid id)
    {
        return await _clienteFornecedorRepository.BuscarPorId(id);
    }

    public async Task<ClienteFornecedor> Adicionar(ClienteFornecedor clienteFornecedor)
    {
        clienteFornecedor.DataInclusao = DateTime.Now;
        clienteFornecedor.StatusCadastro = true;

        try
        {
            clienteFornecedor.Id = Guid.NewGuid();
            await _clienteFornecedorRepository.Adicionar(clienteFornecedor);

            return clienteFornecedor;
        }
        catch(Exception ex)
        {
            throw;
        }

    }

    public async Task<ClienteFornecedor> Atualizar(ClienteFornecedor clienteFornecedor)
    {
        return await _clienteFornecedorRepository.Atualizar(clienteFornecedor);
    }

    public async Task<byte[]> ExportarClientesFornecedoresParaExcel(
    int pageNumber,
    int pageSize,
    string razaoSocial = null,
    string nomeFantasia = null,
    string cpfCnpj = null,
    int? tipoCadastro = null,
    int? tipoPessoa = null,
    bool? status = null)
    {
        var clientesFornecedoresData = await _clienteFornecedorRepository.BuscarTodos(
            pageNumber, pageSize, razaoSocial, nomeFantasia, cpfCnpj,
            tipoCadastro, tipoPessoa, status);

        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Clientes e Fornecedores");

            string[] headers = {
                "Tipo de Cadastro",
                "Tipo de Pessoa",
                "Tipo de Empresa",
                "Porte da Empresa",
                "CPF/CNPJ",
                "RG",
                "Razão Social",
                "Nome Fantasia",
                "Natureza Jurídica",
                "Optante MEI",
                "Optante Simples",
                "Regime Tributário",
                "Inscrição Estadual",
                "Inscrição Municipal",
                "CNAE",
                "Atividade CNAE",
                "Site",
                "Status do Cadastro",
                "Data de Inclusão",
                "Nome do Responsável pela Inclusão",
                "Nome do Responsável pela Última Modificação",
                "Data da Última Modificação",
                "Data de Inativação",
                "Nome do Responsável pela Inativação",
                "Justificativa de Inativação",
                "Observações",
                "Contatos",
                "Endereços"
            };

            for (int col = 1; col <= headers.Length; col++)
            {
                worksheet.Cell(1, col).Value = headers[col - 1];
                worksheet.Cell(1, col).Style.Font.Bold = true;
            }

            int row = 2;
            foreach (var clienteFornecedor in clientesFornecedoresData.Items)
            {
                worksheet.Cell(row, 1).Value = Util.GetTipoCadastroDescription(clienteFornecedor.TipoCadastro);
                worksheet.Cell(row, 2).Value = Util.GetTipoPessoaDescription(clienteFornecedor.TipoPessoa);
                worksheet.Cell(row, 3).Value = Util.GetTipoEmpresaDescription(clienteFornecedor.TipoEmpresa);
                worksheet.Cell(row, 4).Value = Util.GetPorteEmpresaDescription(clienteFornecedor.PorteEmpresa);
                worksheet.Cell(row, 5).Value = Util.FormataCPFCNPJ(clienteFornecedor.CpfCnpj, clienteFornecedor.TipoPessoa);
                worksheet.Cell(row, 6).Value = Util.FormataRG(clienteFornecedor.Rg);
                worksheet.Cell(row, 7).Value = clienteFornecedor.RazaoSocial ?? "N/A";
                worksheet.Cell(row, 8).Value = clienteFornecedor.NomeFantasia ?? "N/A";
                worksheet.Cell(row, 9).Value = Util.GetNaturezaJuridicaDescription(clienteFornecedor.NaturezaJuridica);
                worksheet.Cell(row, 10).Value = (bool)clienteFornecedor.OptanteMEI ? "Sim" : "Não";
                worksheet.Cell(row, 11).Value = (bool)clienteFornecedor.OptanteSimples ? "Sim" : "Não";
                worksheet.Cell(row, 12).Value = Util.GetRegimeTributarioDescription(clienteFornecedor.RegimeTributario);
                worksheet.Cell(row, 13).Value = clienteFornecedor.InscricaoEstadual ?? "N/A";
                worksheet.Cell(row, 14).Value = clienteFornecedor.InscricaoMunicipal ?? "N/A";
                worksheet.Cell(row, 15).Value = Util.FormataCNAE(clienteFornecedor.Cnae);
                worksheet.Cell(row, 16).Value = clienteFornecedor.AtividadeCnae ?? "N/A";
                worksheet.Cell(row, 17).Value = clienteFornecedor.Site ?? "N/A";
                worksheet.Cell(row, 18).Value = clienteFornecedor.StatusCadastro ? "Ativo" : "Inativo";
                worksheet.Cell(row, 19).Value = clienteFornecedor.DataInclusao.ToString("dd/MM/yyyy 'às' HH:mm:ss") ?? "N/A";
                worksheet.Cell(row, 20).Value = clienteFornecedor.NomeRespInclusao ?? "N/A";
                worksheet.Cell(row, 21).Value = clienteFornecedor.NomeRespUltimaModificacao ?? "N/A";
                worksheet.Cell(row, 22).Value = clienteFornecedor.DataUltimaModificacao?.ToString("dd/MM/yyyy 'às' HH:mm:ss") ?? "N/A";
                worksheet.Cell(row, 23).Value = clienteFornecedor.DataInativacao?.ToString("dd/MM/yyyy 'às' HH:mm:ss") ?? "N/A";
                worksheet.Cell(row, 24).Value = clienteFornecedor.NomeRespInativacao ?? "N/A";
                worksheet.Cell(row, 25).Value = clienteFornecedor.JustificativaInativacao ?? "-";
                worksheet.Cell(row, 26).Value = clienteFornecedor.Observacoes ?? "-";
                worksheet.Cell(row, 27).Value = FormatContatos(clienteFornecedor.Contatos);
                worksheet.Cell(row, 28).Value = FormatEnderecos(clienteFornecedor.Enderecos);

                row++;
            }

            worksheet.Columns().AdjustToContents();

            worksheet.Column(27).Style.Alignment.WrapText = true;
            worksheet.Column(28).Style.Alignment.WrapText = true;

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                return stream.ToArray();
            }
        }
    }

    private string FormatContatos(List<Contato> contatos)
    {
        if (contatos == null || !contatos.Any())
            return "-";

        return string.Join("\n\n", contatos.Select(c =>
            $"{c.Nome} ({c.Cargo})\n" +
            $"Email: {c.Email ?? "N/A"}\n" +
            $"Tel: {Util.FormatTelefone(c.Telefone)} {(string.IsNullOrEmpty(c.Ramal) ? "" : $"Ramal: {c.Ramal}")}\n" +
            $"Cel: {Util.FormatTelefone(c.Celular)}"));
    }

    private string FormatEnderecos(List<Endereco> enderecos)
    {
        if (enderecos == null || !enderecos.Any())
            return "-";

        return string.Join("\n\n", enderecos.Select(e =>
            $"{Util.GetTipoEnderecoDescription(e.TipoEndereco)}\n" +
            $"{e.Logradouro}, {e.Numero} {e.Complemento}\n" +
            $"{e.Bairro}, {e.Cidade}/{e.Uf}\n" +
            $"CEP: {Util.FormatCEP(e.Cep)}\n" +
            $"{(string.IsNullOrEmpty(e.Referencia) ? "" : $"Ref: {e.Referencia}")}"));
    }

    public async Task Remover(Guid id)
    {
        await _clienteFornecedorRepository.Remover(id);
    }
}
