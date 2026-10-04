using System.Text.Json.Serialization;

namespace Moviles2.Models
{
    public class PostulanteApi
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("phone")]
        public string Phone { get; set; } = string.Empty;

        public string EstadoTramite { get; set; } = string.Empty;
    }
}
