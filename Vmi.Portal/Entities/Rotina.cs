namespace Vmi.Portal.Entities;

public class Rotina
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nome { get; set; }
    public Guid IdModulo { get; set; }
    public string ModuloNome { get; set; }
}
