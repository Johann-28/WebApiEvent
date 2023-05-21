namespace WebApiEventos.Entities
{
    public class Assistants
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public Users User { get; set; }

        public int EventId { get; set; }
        public Events Event { get; set; }
    }
}
