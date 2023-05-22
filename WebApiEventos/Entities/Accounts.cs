using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebApiEventos.Entities
{
    public class Accounts
    {
        [JsonIgnore]
        public int Id { get; set; }
        [Required(ErrorMessage = "Field Name is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Field email is required")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "Field password is required")]
        public string Password { get; set; }

    }
}
