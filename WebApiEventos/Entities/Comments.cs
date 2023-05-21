﻿namespace WebApiEventos.Entities
{
    public class Comments
    { 
        public int Id { get; set; }
        public int Type { get; set; } //1: Pregunta , 2: Comentario
        public string Comment { get; set; }
        public int UserId { get; set; }
        public Users User { get; set; }
        public int OrgnaizerId { get; set; } // será el id del organizado al que va dirigido el comentario o pregunta
        public Organizers Organizers { get; set; }
    }
}
