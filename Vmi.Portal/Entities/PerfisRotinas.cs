using Vmi.Portal.Enums;

namespace Vmi.Portal.Entities;

public class PerfilRotina
{
    public int Id { get; set; }
    public int PerfilId { get; set; }
    public int RotinaId { get; set; }
    public AcessoEnum AcessoId { get; set; }

    public Perfil Perfil { get; set; }
    public Rotina Rotina { get; set; }
    public Acesso Acesso { get; set; }
}
