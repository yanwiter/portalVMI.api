using static Vmi.Portal.Common.Enums;

namespace Vmi.Portal.Entities
{
    public class ClienteFornecedor
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int TipoCadastro { get; set; }
        public int TipoPessoa { get; set; }
        public int? TipoEmpresa { get; set; }
        public int? PorteEmpresa { get; set; }
        public int? NaturezaJuridica { get; set; }
        public bool? OptanteMEI { get; set; }
        public bool? OptanteSimples { get; set; }
        public string CpfCnpj { get; set; }
        public string Rg { get; set; }
        public string RazaoSocial { get; set; }
        public string NomeFantasia { get; set; }
        public int RegimeTributario { get; set; }
        public string InscricaoEstadual { get; set; }
        public string InscricaoMunicipal { get; set; }
        public string Cnae { get; set; }
        public string AtividadeCnae { get; set; }
        public string Site { get; set; }
        public bool StatusCadastro { get; set; }
        public DateTime DataInclusao { get; set; }
        public Guid IdRespInclusao { get; set; }
        public string NomeRespInclusao { get; set; }
        public DateTime? DataInativacao { get; set; }
        public Guid IdRespUltimaModificacao { get; set; }
        public string NomeRespUltimaModificacao { get; set; }
        public DateTime? DataUltimaModificacao { get; set; }
        public Guid? IdRespInativacao { get; set; }
        public string JustificativaInativacao { get; set; }
        public string NomeRespInativacao { get; set; }
        public string Observacoes { get; set; }

        // Relacionamentos
        public List<Endereco> Enderecos { get; set; } = new List<Endereco>();
        public List<Contato> Contatos { get; set; } = new List<Contato>();
        public List<DadoFinanceiro> DadosFinanceiros { get; set; } = new List<DadoFinanceiro>();
        public List<Anexo> Anexos { get; set; } = new List<Anexo>();
    }
}
