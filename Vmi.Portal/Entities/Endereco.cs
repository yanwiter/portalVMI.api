using static Vmi.Portal.Common.Enums;

namespace Vmi.Portal.Entities;
public class Endereco
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid IdClienteFornecedor { get; set; }
    public string Cep { get; set; }
    public string Logradouro { get; set; }
    public int TipoEndereco { get; set; }
    public string Complemento { get; set; }
    public string Numero { get; set; }
    public string Cidade { get; set; }
    public string Bairro { get; set; }
    public string Uf { get; set; }
    public string Referencia { get; set; }

    public ClienteFornecedor ClienteFornecedor { get; set; }
}
