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
    [Authorize(Policy = "UserPolicy")]
    public class UsersController : ControllerBase
    {
        
        private readonly UsersService usersService;
       

        // Inicializa una nueva instancia de la clase UsersController.
        // Parámetros:
        //   - dbContext: Contexto de la base de datos de la aplicación.
        public UsersController(UsersService usersService)
        {
            
            this.usersService = usersService;
    
        }

        // Obtiene una lista de eventos próximos para el usuario actual.
        // Retorna:
        //   - Una lista de objetos EventsDto que representan los eventos próximos(7 dias o menos a suceder).
        [HttpGet("upcomingEvents")]
        public async Task<ActionResult<List<EventsDto>>> UpComing()
        {

            //Consiguiendo id del usuario
            int userId = int.Parse((HttpContext.User.FindFirst("UserId")).Value);

            var eventsDtoList = await usersService.UpComing(userId);

            return eventsDtoList;

        }


        // Obtiene todos los usuarios.
        // Retorna:
        //   - Una lista de objetos Users que representan todos los usuarios registrados.

        [HttpGet("get")]
        public async Task<ActionResult<List<Users>>> GetAll()
        {

            var events = await usersService.GetAll();
            return events;

        }


        // Agrega un evento a la lista de favoritos del usuario actual.
        // Parámetros:
        //   - eventId: ID del evento a agregar a favoritos.
        // Retorna:
        //   - Un objeto IActionResult que indica si se agregó el evento correctamente o el motivo del error.

        [HttpGet("favorites/{eventId}")]
        public async Task<IActionResult> AddToFavorites( int eventId)
        {

            //Consiguiendo id del usuario
            int userId = int.Parse((HttpContext.User.FindFirst("UserId")).Value);

            var isValid = await usersService.IsValid(userId, eventId);
          
            
            if (isValid.Equals("True"))
            {
                await usersService.AddToFavoritesService(eventId ,userId);
                return Ok(new {message = "Event added succesfully "});
            }
            return BadRequest(isValid);
          
        }

        // Sigue a un organizador agregándolo a la lista de organizaciones seguidas por el usuario actual.
        // Parámetros:
        //   - organizatorId: ID del organizador a seguir.
        // Retorna:
        //   - Un objeto IActionResult que indica si se siguió al organizador correctamente o el motivo del error.
        [HttpGet("follow/{organizatorId}")]
        public async Task<IActionResult> FollowOrganizator(int organizatorId)
        {

            //Consiguiendo id del usuario
            int userId = int.Parse((HttpContext.User.FindFirst("UserId")).Value);

            //Determinando si los datos son correctos
            var isValid = await usersService.OrganizerValid(userId, organizatorId);


            if (isValid.Equals("True"))
            {
                await usersService.FollowOrganizerService(organizatorId, userId);
                return Ok(new { message = "Followed successfully" });
            }
            return BadRequest(isValid);

        }

        // Obtiene una lista de cupones válidos para el usuario actual.
        // Retorna:
        //   - Una lista de objetos CouponsDto que representan los cupones válidos.
        [HttpGet("coupons")]
        public async Task<IEnumerable<CouponsDto>> Coupons()
        {
            //Consiguiendo id del usuario
            int userId = int.Parse((HttpContext.User.FindFirst("UserId")).Value);

            var coupons = await usersService.CouponsService(userId);

            return coupons;
        }


    }
}
