using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WebApiEventos.DTOs;
using WebApiEventos.Entities;
using WebApiEventos.Services;

namespace WebApiEventos.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class AssistantsController : ControllerBase
    {
        private readonly EventsService eventsService;
        private readonly UsersService usersService;
        private readonly AssistantsService assistantsService;
        private readonly ApplicationDbContext dbContext;

        // Inicializa una nueva instancia de la clase AssistantsController.
        // Parámetros:
        //   - eventsService: Instancia del servicio EventsService.
        //   - usersService: Instancia del servicio UsersService.
        //   - assistantsService: Instancia del servicio AssistantsService.
        //   - dbContext: Contexto de la base de datos de la aplicación.
        public AssistantsController(EventsService eventsService, UsersService usersService, AssistantsService assistantsService, ApplicationDbContext dbContext)
        {
            this.eventsService = eventsService;
            this.usersService = usersService;
            this.assistantsService = assistantsService;
            this.dbContext = dbContext;
        }

       

        // Obtiene una lista de asistentes en formato DTO.
        // Retorna:
        //   - Una colección de objetos AssistantsDto que representan a los asistentes en formato DTO.
        [HttpGet("get")]
        public async Task<IEnumerable<AssistantsDto>> GetDto()
        {
            return await assistantsService.Get();
        }

        // Permite a los usuarios registrarse para un evento específico, manteniendo un registro de los asistentes y la cantidad de plazas disponibles.
        // Antes de registrar al usuario, se realiza una validación para verificar si el registro es válido.
        // Si la validación no pasa, se devuelve un BadRequest con un mensaje de error.
        // Después de registrar al usuario, se devuelve un Accepted indicando que el registro se actualizó exitosamente.
        // Parámetros:
        //   - userId: ID del usuario que se desea registrar como asistente.
        //   - eventId: ID del evento al que se desea registrar el usuario.
        // Retorna:
        //   - Un objeto IActionResult que indica el resultado de la operación.
        [Authorize(Policy = "UserPolicy")]
        [HttpPost("register")]
        public async Task<IActionResult> Register(int eventId)
        {


            //Consiguiendo id del usuario
            int userId = int.Parse((HttpContext.User.FindFirst("UserId")).Value);


            string result = await assistantsService.Validate(userId, eventId);

            if (result != "Valid")
            {
                return BadRequest(result);
            }

            var evento = await eventsService.GetById(eventId);

            //Añadiendo verificacion de fecha
            if(evento.Date < DateTime.Now)
            {
                return BadRequest(new { message = "Event already passed" });
            }

            var assistant = await assistantsService.Create(userId, eventId);

            return Accepted(new { message = "Registro actualizado con éxito" });
        }
    }
}
