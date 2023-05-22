using System.ComponentModel.DataAnnotations;

namespace WebApiEventos.Entities
{
    public class Comments
    { 
        public int Id { get; set; }

        [Required(ErrorMessage = "Field Type required")]
        public int Type { get; set; } //1: Pregunta , 2: Comentario

        [Required(ErrorMessage = "Field Comment required")]
        public string Comment { get; set; }

        [Required(ErrorMessage = "Field userId required")]
        public int UserId { get; set; }

        public Users User { get; set; }

        [Required(ErrorMessage = "Field organizerId required")]
        public int OrgnaizerId { get; set; } // será el id del organizado al que va dirigido el comentario o pregunta

        public Organizers Organizers { get; set; }
    }
}
