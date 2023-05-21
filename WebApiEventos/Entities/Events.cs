namespace WebApiEventos.Entities
{
    public class Events
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Descripcion { get; set; }

        public DateTime Date { get; set; }

        public string Ubicacion { get; set; }

        public int Capacidad { get; set; }
        public ICollection<Assistants> Assistants { get; set; }
        public int OrganizersId { get; set; }
        public Organizers Organizers { get; set; }
    }
}
