namespace WebApiEventos.DTOs
{
    public class CommentsDto
    {
        public string Name { get; set; }
        public string Type { get; set; } //1: Pregunta , 2: Comentario
        public string Comment { get; set; }
        public string Organizer { get; set; }
    }
}
