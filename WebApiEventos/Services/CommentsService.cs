using Microsoft.EntityFrameworkCore;
using WebApiEventos.DTOs;

namespace WebApiEventos.Services
{
    // Servicio para gestionar las operaciones relacionadas con los comentarios.
    public class CommentsService
    {
        private readonly ApplicationDbContext dbContext;

        // Inicializa una nueva instancia de la clase CommentsService.
        // Parámetros:
        //   - dbContext: Contexto de la base de datos de la aplicación.
        public CommentsService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        // Obtiene todos los comentarios en formato DTO.
        // Retorna:
        //   - Una colección de objetos CommentDto que representan los comentarios en formato DTO.
        public async Task<IEnumerable<CommentsDto>> Get()
        {
            return await dbContext.Comments.Select(c => new CommentsDto
            {
                Name = c.User.Name,
                Type = c.Type == 1 ? "Pregunta" : "Comentario",
                Comment = c.Comment
            }).ToListAsync();
        }
    }
}
