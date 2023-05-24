using Microsoft.AspNetCore.Mvc;
using WebApiEventos.Entities;
using WebApiEventos.Services;
using WebApiEventos.DTOs;
using Microsoft.AspNetCore.Authorization;


namespace WebApiEventos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {

       
        private readonly EventsService eventsService;
        public EventsController(EventsService eventsService)
        {

            this.eventsService = eventsService;
        }

     
        //Regresa todos los registros de la tabla eventos
        [HttpGet("events")]
        public async Task<IEnumerable<EventsDto>> GetAll()
        {
            return await eventsService.Get();
        }

 
        // Obtiene una lista de eventos en formato DTO (Data Transfer Object).
        // Permite mostrar una lista de todos los eventos creados, proporcionando información básica como el nombre, fecha y ubicación.
        // Esto permite a los usuarios explorar los eventos y decidir a cuál desean asistir.

        [HttpGet("trending")]
        public async Task<IEnumerable<EventsDto>> Trending()
        {
            return await eventsService.GetTop();
        }


        //Recibe un objeto con la fecha, nombre y ubicacion y se filtran
        [HttpPost("search")]
        public async Task<IActionResult> Search(EventSearchDtoIn searchBy)
        {
 
            var result = await eventsService.SearchEvent(searchBy);
            return Ok(result);
        
        }

        // Crea un nuevo evento.
        // Permite a los usuarios crear un evento especificando el nombre, descripción, fecha, hora, ubicación y capacidad máxima de asistentes.
        // Parámetros:
        // - evento: Objeto que contiene los detalles del evento a crear.
        // Retorna:
        // - Respuesta HTTP indicando si se creó el evento exitosamente.
        [Authorize(Policy = "OrganizerPolicy")]
        [HttpPost("create")]
        public async Task<IActionResult> Create(EventDtoIn eventToRegister)
        {
            Events evento = new Events();

            evento.Name = eventToRegister.Name;
            evento.Descripcion = eventToRegister.Description;
            evento.Date = eventToRegister.Date;
            evento.Ubicacion = eventToRegister.Ubication;
            evento.Capacidad = eventToRegister.Capacity;

            if (evento.Date < DateTime.Now)
                return BadRequest(new { message = "The date expired"});

            if(evento.Capacidad < 1 )
            {
                return BadRequest(new { message = "The capacity must be over 0" });
            }

            //Consiguiendo id del usuario
            int organizerId = int.Parse((HttpContext.User.FindFirst("UserId")).Value);

            evento.OrganizersId = organizerId;
            await eventsService.Create(evento);

            return Ok(new { message = "Event succesfully created"});
        }

        //Permitir al organizador del evento editar los detalles del evento en cualquier momento. 
        [Authorize(Policy = "OrganizerPolicy")]
        [HttpPut("udpate/{id}")]
        public async Task<ActionResult> Update(Events evento, int id)
        {

            //Consiguiendo id del usuario
            int userId = int.Parse((HttpContext.User.FindFirst("UserId")).Value);

            if(evento.Id != id)
            {
                return BadRequest(new { message = $"Url Id ({id}) doesnt match object Id({evento.Id})" });
            }


            if(userId != evento.OrganizersId)
            {
                return BadRequest(new { message = "You are not the organizer of this event" });
            }


            var eventToUpdate = await eventsService.GetById(evento.Id);

            if (eventToUpdate is not null)
            {
                await eventsService.UpdateService(evento.Id, evento);
                return Ok(new { message = $"Event succesfully updated" });
            }

            return BadRequest(new { message ="Event doesnt exists"});
        }
     
    }
}
