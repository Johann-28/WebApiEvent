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
        [Required]
        [MaxLength(140)]
        public string Descripcion { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public string Ubicacion { get; set; }
        [Required]
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
