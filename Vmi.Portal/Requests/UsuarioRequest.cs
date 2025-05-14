namespace Vmi.Portal.Requests
{
    public class UsuarioRequest
    {
        public int UsuarioID { get; set; }

        public string Nome { get; set; }

        public string Email { get; set; }

        public string Senha { get; set; }

        public int? PerfilID { get; set; }

        public bool? StatusUsuario { get; set; }
    }
}
