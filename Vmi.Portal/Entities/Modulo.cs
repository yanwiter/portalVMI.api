namespace Vmi.Portal.Entities;

public class Modulo
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nome { get; set; }
    public string? Descricao { get; set; }
}
