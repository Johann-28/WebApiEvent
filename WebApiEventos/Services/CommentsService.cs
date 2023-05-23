using Microsoft.EntityFrameworkCore;
using WebApiEventos.DTOs;
using WebApiEventos.Entities;

namespace WebApiEventos.Services
{
    // Servicio para gestionar las operaciones relacionadas con los comentarios.
    public class CommentsService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly OrganizersService organizersService;

        // Inicializa una nueva instancia de la clase CommentsService.
        // Parámetros:
        //   - dbContext: Contexto de la base de datos de la aplicación.
        public CommentsService(ApplicationDbContext dbContext, OrganizersService organizersService)
        {
            this.dbContext = dbContext;
            this.organizersService = organizersService;
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
                    Comment = c.Comment,
                    Organizer = c.Organizers.Name
                }).ToListAsync();
        }

        // Agrega un comentario a la base de datos.
        // Parámetros:
        //   - comentario: Objeto Comment que representa el comentario a agregar.
        //   - userId: ID del usuario que realiza el comentario.
        public async Task PostService(Comments comentario, int userId)
        {

            // Agrega el comentario a la base de datos.
            dbContext.Comments.Add(comentario);
            await dbContext.SaveChangesAsync();

        }


        // Obtiene todos los comentarios que ha recibido un organizador en formato DTO.
        // Parámetros:
        //   - OrganizerId: ID del organizador cuyos comentarios se desean obtener.
        // Retorna:
        //   - Una colección de objetos CommentsDto que representan los comentarios del organizador en formato DTO.
        public async Task<IEnumerable<CommentsDto>> GetOrganizerComments(int OrganizerId)
        {
            return await dbContext.Comments
                .Where(c => c.OrgnaizerId == OrganizerId)
                .Select(c => new CommentsDto
                {
                    Name = c.User.Name,
                    Type = c.Type == 1 ? "Pregunta" : "Comentario",
                    Comment = c.Comment,
                    Organizer = c.Organizers.Name
                })
                .ToListAsync();
        }

    }
}
