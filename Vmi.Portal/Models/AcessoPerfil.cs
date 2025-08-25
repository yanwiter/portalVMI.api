namespace Vmi.Portal.Models;

public class AcessoPerfil
{
    public Guid IdAcesso { get; set; }
    public string Acesso { get; set; }
    public Guid IdRotina { get; set; }
    public string Rotina { get; set; }
    public Guid IdModulo { get; set; }
    public string Modulo { get; set; }
    public bool Ativo { get; set; }
}
