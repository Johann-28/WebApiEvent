using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using WebApiEventos.DTOs;
using WebApiEventos.Entities;
using WebApiEventos.Services;

namespace WebApiEventos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentsController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly CommentsService service;
        private readonly UsersService usersService;
        private readonly OrganizersService organizationizersService;
        public CommentsController(ApplicationDbContext dbContext, CommentsService service, UsersService usersService, OrganizersService organizationizersService)
        {
            this.dbContext = dbContext;
            this.service = service;
            this.usersService = usersService;
            this.organizationizersService = organizationizersService;
        }

        [HttpGet("get")]
        public async Task<IEnumerable<CommentsDto>> Get()
        {

            return await service.Get();
        }

      

        // Permite a los usuarios enviar preguntas o comentarios al organizador del evento.
        [HttpPost("post")]
        public async Task<IActionResult> Post(Comments comentario)
        {
            // Verifica si el tipo de comentario es válido (1: Pregunta, 2: Comentario).
            if (comentario.Type != 1 && comentario.Type != 2)
            {
                return BadRequest(new { message = "Enter 1 for a question or 2 for a comment" });
            }

            var user = await dbContext.Users.FindAsync(comentario.UserId);
            if(user is null)
            {
                return BadRequest(new { message = "User doesnt exists" });
            }

            var organizer = await organizationizersService.GetById(comentario.OrgnaizerId);
            if(organizer is null)
            {
                return BadRequest(new { message = $"Organizer {comentario.OrgnaizerId} doesnt exists" });
            }

            comentario.Organizers = organizer;
            comentario.User = user;

            // Agrega el comentario a la base de datos.
            dbContext.Comments.Add(comentario);
            await dbContext.SaveChangesAsync();

            // Retorna una respuesta exitosa junto con un mensaje.
            return Ok(new { message = "Post created successfully" });
        }

    }
}
