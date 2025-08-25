namespace Vmi.Portal.Requests
{
    public class UsuarioRequest
    {
        public int UsuarioID { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public Guid? IdPerfil{ get; set; }
        public bool? StatusUsuario { get; set; }
        public int? TipoSuspensao { get; set; }
        public DateTime? DataInicioSuspensao { get; set; }
        public DateTime? DataFimSuspensao { get; set; }
        public string MotivoSuspensao { get; set; }
        public Guid? IdRespSuspensao { get; set; }
        public string NomeRespSuspensao { get; set; }
        public DateTime? DataSuspensao { get; set; }
    }
}
