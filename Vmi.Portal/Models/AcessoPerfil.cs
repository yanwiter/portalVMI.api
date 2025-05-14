using Vmi.Portal.Enums;

namespace Vmi.Portal.Models;

public class AcessoPerfil
{
    public AcessoEnum AcessoId { get; set; }
    public string Acesso { get; set; }
    public int RotinaId { get; set; }
    public string Rotina { get; set; }
    public int ModuloId { get; set; }
    public string Modulo { get; set; }
    public bool Ativo { get; set; }
}
