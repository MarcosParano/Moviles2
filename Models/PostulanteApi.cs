using System.Text.Json.Serialization;

namespace Moviles2.Models
{
    public class PostulanteApi
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }
}
