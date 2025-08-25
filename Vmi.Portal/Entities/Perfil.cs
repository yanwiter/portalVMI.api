using Vmi.Portal.Enums;

namespace Vmi.Portal.Entities;

public class Perfil
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nome { get; set; }
    public string Descricao { get; set; }
    public StatusPerfilEnum StatusPerfil { get; set; } = StatusPerfilEnum.Ativo;
    public DateTime? DataInclusao { get; set; }
    public Guid IdRespInclusao { get; set; }
    public string NomeRespInclusao { get; set; }
    public Guid IdRespUltimaModificacao { get; set; }
    public string NomeRespUltimaModificacao { get; set; }
    public DateTime? DataUltimaModificacao { get; set; }
    public Guid? IdRespInativacao { get; set; }
    public DateTime? DataInativacao { get; set; }
    public string NomeRespInativacao { get; set; }
    public string JustificativaInativacao { get; set; }
    public TipoSuspensaoEnum? TipoSuspensao { get; set; } = TipoSuspensaoEnum.Temporaria;
    public DateTime? DataInicioSuspensao { get; set; }
    public DateTime? DataFimSuspensao { get; set; }
    public string MotivoSuspensao { get; set; }
    public Guid? IdRespSuspensao { get; set; }
    public string NomeRespSuspensao { get; set; }
    public DateTime? DataSuspensao { get; set; }
}
