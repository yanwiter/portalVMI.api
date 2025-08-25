using static Vmi.Portal.Common.Enums;

namespace Vmi.Portal.Entities;

public class DadoFinanceiro
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid IdClienteFornecedor { get; set; }
    public decimal LimiteCredito { get; set; }
    public CondicaoPagamentoEnum CondicaoPagamento { get; set; }
    public BancosEnum Banco { get; set; }
    public string Agencia { get; set; }
    public string Conta { get; set; }
    public TipoContaEnum TipoConta { get; set; }
    public string ChavePix { get; set; }

    public ClienteFornecedor ClienteFornecedor { get; set; }

}
