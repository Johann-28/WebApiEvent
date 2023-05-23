using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApiEventos.DTOs;
using WebApiEventos.Entities;
using WebApiEventos.Services;

namespace WebApiEventos.Controllers
{
    // Controlador para gestionar las operaciones relacionadas con los organizadores.
    [ApiController]
    [Authorize(Policy = "OrganizerPolicy")]
    public class OrganizersController : ControllerBase
    {
     
        private readonly OrganizersService service;
        private readonly CommentsService commentsService;

        public OrganizersController(OrganizersService service, CommentsService commentsService)
        {
 
            this.service = service;
            this.commentsService = commentsService;
          
        }

        // Obtiene todos los organizadores.
        // Retorna:
        //   - Una colección de objetos Organizers que representan a todos los organizadores.
        [HttpGet("get")]
        public async Task<IEnumerable<Organizers>> Get()
        {
            return await service.Get();
        }


        // Obtiene los comentarios de un organizador.
        // Retorna:
        //   - Una colección de objetos CommentsDto que representan los comentarios del organizador autenticado.
        [HttpGet("getComments")]
        public async Task<IEnumerable<CommentsDto>> GetComments()
        {
            //Consiguiendo id del usuario
            int organizerId = int.Parse((HttpContext.User.FindFirst("UserId")).Value);
            return await commentsService.GetOrganizerComments(organizerId);
        }



    }
}
