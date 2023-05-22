namespace WebApiEventos.Entities
{
    public class Users
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public ICollection<Assistants> Asistants { get; set; }
        public ICollection<Comments> Comments { get; set; }
        public ICollection<Events> Favorites { get; set; }
        public ICollection<Organizers> Organizations { get; set; }

    }
}
