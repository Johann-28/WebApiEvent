using System.ComponentModel.DataAnnotations;

namespace WebApiEventos.Entities
{
    public class Assistants
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo de userId es requerido")]
        public int UserId { get; set; }
        public Users User { get; set; }
        [Required(ErrorMessage = "El campo de eventId es requerido")]
        public int EventId { get; set; }
        public Events Event { get; set; }
    }
}
