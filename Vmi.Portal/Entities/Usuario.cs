namespace Vmi.Portal.Entities;

public class Usuario
{
    public int Id { get; set; }

    public string Nome { get; set; }

    public string Email { get; set; }

    public string Senha { get; set; }

    public int? Perfil_id { get; set; }

    public string PerfilNome { get; set; }

    public bool StatusUsuario { get; set; }

    public bool? IsPrimeiroAcesso { get; set; }

    public int IdRespInclusao { get; set; }

    public string NomeRespInclusao { get; set; }

    public DateTime? DataInclusao { get; set; }

    public int IdRespUltimaAlteracao { get; set; }

    public string NomeRespUltimaAlteracao { get; set; }

    public DateTime? DataUltimaAlteracao { get; set; }

    public int IdRespInativacao { get; set; }

    public string NomeRespInativacao { get; set; }

    public DateTime? DataInativacao { get; set; }

    public string JustificativaInativacao { get; set; }

}
