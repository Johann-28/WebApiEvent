using WebApiEventos.Entities;
using WebApiEventos.DTOs.UserDto;

namespace WebApiEventos.DTOs
{
    public class UsersDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public ICollection<UsersEventsDto> Assistants { get; set; }
        public ICollection<UsersCommentsDto> Comments { get; set; }
        public ICollection<UsersEventsDto> Favorites { get; set; }
        public ICollection<UsersOrganizersFollowed> Organizers { get; set; }


    }
}
