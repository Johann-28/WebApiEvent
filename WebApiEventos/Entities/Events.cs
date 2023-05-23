using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebApiEventos.Entities
{
    public class Events
    {
        
        public int Id { get; set; }
        [Required(ErrorMessage = "Name field required")]
        public string Name { get; set; }
        [Required]
        [MaxLength(100, ErrorMessage = "The maximum length of the Description field is 100 characters")]
        public string Descripcion { get; set; }
        [Required(ErrorMessage = "Date field required")]
        public DateTime Date { get; set; }
        [Required(ErrorMessage = "Ubication field required")]
        public string Ubicacion { get; set; }
        [Required(ErrorMessage = "Capacity field required")]
        public int Capacidad { get; set; }

        [JsonIgnore]
        public ICollection<Assistants> Assistants { get; set; }

        public int OrganizersId { get; set; }
        [JsonIgnore]
        public Organizers Organizers { get; set; }
        [JsonIgnore]
        public ICollection<Coupons> Coupons { get; set; }
    }
}
