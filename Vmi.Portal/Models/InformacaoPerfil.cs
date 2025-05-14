using Vmi.Portal.Entities;
using Vmi.Portal.Enums;

namespace Vmi.Portal.Models;

public class InformacaoPerfil
{
    public Perfil Perfil { get; set; }
    public IEnumerable<AcessoPerfil> Acessos { get; set; }
}
