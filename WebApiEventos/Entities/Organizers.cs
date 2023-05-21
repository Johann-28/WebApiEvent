namespace WebApiEventos.Entities
{
    public class Organizers
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Events> Events { get; set; }
    }
}
