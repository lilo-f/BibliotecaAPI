using System.Text.Json.Serialization;

namespace BibliotecaAPI.Models
{
    public class Usuario
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
    }

}
