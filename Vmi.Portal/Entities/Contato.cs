using static Vmi.Portal.Common.Enums;

namespace Vmi.Portal.Entities;

public class Contato
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid IdClienteFornecedor { get; set; }
    public string Nome { get; set; }
    public string Cargo { get; set; }
    public string Email { get; set; }
    public string Telefone { get; set; }
    public string Celular { get; set; }
    public string Ramal { get; set; }

    public ClienteFornecedor ClienteFornecedor { get; set; }
}
