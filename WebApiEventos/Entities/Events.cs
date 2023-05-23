using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebApiEventos.Entities
{
    public class Events
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        public string Descripcion { get; set; }

        public DateTime Date { get; set; }

        public string Ubicacion { get; set; }

        public int Capacidad { get; set; }

        [JsonIgnore]
        public ICollection<Assistants> Assistants { get; set; }

        [JsonIgnore]
        public int OrganizersId { get; set; }
        [JsonIgnore]
        public Organizers Organizers { get; set; }
        [JsonIgnore]
        public ICollection<Coupons> Coupons { get; set; }
    }
}
