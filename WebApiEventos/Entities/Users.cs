using System.ComponentModel.DataAnnotations;
using WebApiEventos.DTOs;

namespace WebApiEventos.Entities
{
    public class Users
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name field required")]
        [MaxLength(50, ErrorMessage = "The maximum length of the Name field is 50 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email Field Required")]
        [MaxLength(100, ErrorMessage = "The maximum length of the Email field is 100 characters")]
        [EmailAddress(ErrorMessage = "The Email field does not have a valid email format")]

        public string Email { get; set; }

        [Required(ErrorMessage = "Password field required")]
        [MaxLength(20, ErrorMessage = "The maximum length of the Password field is 20 characters.")]

        public string Password { get; set; }

        public ICollection<Assistants> Asistants { get; set; }
        public ICollection<Comments> Comments { get; set; }
        public ICollection<Events> Favorites { get; set; }
        public ICollection<Organizers> Organizations { get; set; }

    }
}
