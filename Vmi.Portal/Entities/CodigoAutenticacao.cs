namespace Vmi.Portal.Entities
{
    public class CodigosAutenticacao
    {
        public Guid Id { get; set; }
        public Guid IdUsuario { get; set; }
        public string Codigo { get; set; }
        public DateTime DataExpiracao { get; set; }
        public DateTime DataCriacao { get; set; }
    }
}
