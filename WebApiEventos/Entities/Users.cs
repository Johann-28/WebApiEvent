using System.ComponentModel.DataAnnotations;
using WebApiEventos.DTOs;

namespace WebApiEventos.Entities
{
    public class Users
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo Name es requerido")]
        [MaxLength(50, ErrorMessage = "La longitud máxima del campo Name es de 50 caracteres")]
        public string Name { get; set; }

        [Required(ErrorMessage = "El campo Email es requerido")]
        [MaxLength(100, ErrorMessage = "La longitud máxima del campo Email es de 100 caracteres")]
        [EmailAddress(ErrorMessage = "El campo Email no tiene un formato de correo electrónico válido")]


        public string Email { get; set; }

        [Required(ErrorMessage = "El campo Password es requerido")]
        [MaxLength(20, ErrorMessage = "La longitud máxima del campo Password es de 20 caracteres")]

        public string Password { get; set; }

        public ICollection<Assistants> Asistants { get; set; }
        public ICollection<Comments> Comments { get; set; }
        public ICollection<Events> Favorites { get; set; }
        public ICollection<Organizers> Organizations { get; set; }

    }
}
