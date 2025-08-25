namespace Vmi.Portal.Entities;

public class PerfilRotina
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid IdPerfil { get; set; }
    public Guid IdRotina { get; set; }
    public Guid IdAcesso { get; set; }

    public Perfil Perfil { get; set; }
    public Rotina Rotina { get; set; }
    public Acesso Acesso { get; set; }
}
