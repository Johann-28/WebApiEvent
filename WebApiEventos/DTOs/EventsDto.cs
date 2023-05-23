namespace WebApiEventos.DTOs
{
    public class EventsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        public string Date { get; set; }
        public string Hour { get; set; }

        public string Ubication { get; set; }

        public string Organizer { get; set; }
        public int Capacity { get; set; }
    }
}
