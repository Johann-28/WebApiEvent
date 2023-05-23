using System.ComponentModel.DataAnnotations;

namespace WebApiEventos.Entities
{
    public class Organizers
    {

        public int Id { get; set; }
        [Required(ErrorMessage = "Name field required")]
        public string Name { get; set; }
        public ICollection<Events> Events { get; set; }
    }
}
