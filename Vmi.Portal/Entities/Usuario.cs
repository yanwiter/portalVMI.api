using Vmi.Portal.Enums;

namespace Vmi.Portal.Entities;

public class Usuario
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nome { get; set; }
    public string Email { get; set; }
    public string Senha { get; set; }
    public Guid? IdPerfil { get; set; }
    public string PerfilNome { get; set; }
    public StatusUsuarioEnum StatusUsuario { get; set; } = StatusUsuarioEnum.Ativo; 
    public bool? IsPrimeiroAcesso { get; set; }
    public Guid IdRespInclusao { get; set; }
    public string NomeRespInclusao { get; set; }
    public DateTime? DataInclusao { get; set; }
    public Guid IdRespUltimaAlteracao { get; set; }
    public string NomeRespUltimaAlteracao { get; set; }
    public DateTime? DataUltimaAlteracao { get; set; }
    public Guid IdRespInativacao { get; set; }
    public string NomeRespInativacao { get; set; }
    public DateTime? DataInativacao { get; set; }
    public string JustificativaInativacao { get; set; }
    public string Telefone { get; set; }
    public string CpfCnpj { get; set; }
    public string UsuarioLogin { get; set; }
    public DateTime? DataExpiracao { get; set; }
    public string Observacoes { get; set; }
    public int? TipoAcesso { get; set; }
    public int? TipoPessoa { get; set; }
    public string HorariosAcesso { get; set; }
    public DateTime? DataUltimoLogin { get; set; }
    public string? FotoPerfil { get; set; }
    public TipoSuspensaoEnum? TipoSuspensao { get; set; } = TipoSuspensaoEnum.Temporaria;
    public DateTime? DataInicioSuspensao { get; set; }
    public DateTime? DataFimSuspensao { get; set; }
    public string MotivoSuspensao { get; set; }
    public Guid? IdRespSuspensao { get; set; }
    public string NomeRespSuspensao { get; set; }
    public DateTime? DataSuspensao { get; set; }
}