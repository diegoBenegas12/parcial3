using api.primerparcial.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace api.primerparcial.Models
{
    public class Clientes
    {
        public int Id { get; set; }
        public int IdCiudad { get; set; }
        public string? Nombres { get; set; }
        public string? Apellidos { get; set; }
        public string? Documento { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string? Ciudad { get; set; }
        public string? Nacionalidad { get; set; }

        [ForeignKey("IdCiudad")]

        public Ciudades? Ciudades { get; set; }
    }
}
