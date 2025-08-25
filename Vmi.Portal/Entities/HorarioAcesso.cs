using Newtonsoft.Json;

namespace Vmi.Portal.Entities
{
    public class HorarioAcesso
    {
        [JsonProperty("diaSemana")]
        public int DiaSemana { get; set; }

        [JsonProperty("inicio")]
        public string Inicio { get; set; }

        [JsonProperty("fim")]
        public string Fim { get; set; }
    }
}
