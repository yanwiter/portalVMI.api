namespace Vmi.Portal.Entities;

public class Perfil
{
    public int Id { get; set; }

    public string Nome { get; set; }

    public string Descricao { get; set; }

    public bool StatusPerfil { get; set; }

    public DateTime? DataInclusao { get; set; }

    public int IdRespInclusao { get; set; }

    public string NomeRespInclusao { get; set; }

    public int IdRespUltimaModificacao { get; set; }

    public string NomeRespUltimaModificacao { get; set; }

    public DateTime? DataUltimaModificacao { get; set; }

    public int? IdRespInativacao { get; set; }

    public DateTime? DataInativacao { get; set; }

    public string NomeRespInativacao { get; set; }

    public string JustificativaInativacao { get; set; }
}
