using Vmi.Portal.Entities;

namespace Vmi.Portal.Models;

public class InformacaoPerfil
{
    public Perfil Perfil { get; set; }
    public IEnumerable<AcessoPerfil> Acessos { get; set; }
}
