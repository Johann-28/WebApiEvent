using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApiEventos.DTOs;
using WebApiEventos.Entities;
using WebApiEventos.Services;

namespace WebApiEventos.Controllers
{

    // Controlador para gestionar las operaciones relacionadas con los usuarios.
    [ApiController]
    [Route("api/[controller]")]
    public class CommentsController : ControllerBase
    {
     
        private readonly CommentsService commentsService;
        private readonly OrganizersService organizationizersService;
        private readonly UsersService usersService;
        public CommentsController(CommentsService commentsService,  OrganizersService organizationizersService, UsersService usersService)
        {
       
            this.commentsService = commentsService;
            this.organizationizersService = organizationizersService;
            this.usersService = usersService;
        }

        // Obtiene todos los comentarios en formato DTO.
        // Retorna:
        //   - Una colección de objetos CommentsDto que representan los comentarios en formato DTO.
        [HttpGet("get")]
        public async Task<IEnumerable<CommentsDto>> Get()
        {

            return await commentsService.Get();
        }


        // Permite a los usuarios enviar preguntas o comentarios al organizador del evento.
        // Requiere autenticación del usuario.
        // Parámetros:
        //   - comentario: Objeto Comment que representa el comentario a enviar.
        // Retorna:
        //   - Un IActionResult que indica el resultado de la operación.
        [Authorize(Policy = "UserPolicy")]
        [HttpPost("post")]
        public async Task<IActionResult> Post(Comments comentario)
        {

            //Consiguiendo id del usuario
            int userId = int.Parse((HttpContext.User.FindFirst("UserId")).Value);

            // Verifica si el tipo de comentario es válido (1: Pregunta, 2: Comentario).
            if (comentario.Type != 1 && comentario.Type != 2)
            {
                return BadRequest(new { message = "Enter 1 for a question or 2 for a comment" });
            }

            var user = await usersService.GetById(userId);
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
            await commentsService.PostService(comentario, userId);

            // Retorna una respuesta exitosa junto con un mensaje.
            return Ok(new { message = "Post created successfully" });
        }

    }
}
