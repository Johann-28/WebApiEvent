using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiEventos.Entities;
using WebApiEventos.Services;
using WebApiEventos.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Net.Mail;
using System.Net;

namespace WebApiEventos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {

        private readonly ApplicationDbContext dbContext;
        private readonly EventsService eventsService;
        private readonly OrganizersService organizersService;
        public EventsController(ApplicationDbContext dbContext, EventsService eventsService, OrganizersService organizersService)
        {

            this.dbContext = dbContext;
            this.eventsService = eventsService;
            this.organizersService = organizersService;
        }

     
        //Regresa todos los registros de la tabla eventos
        [HttpGet("events")]
        public async Task<IEnumerable<EventsDto>> GetAll()
        {

            //la funcion Include hace un Join en base al identificador propio y la tabla Asistants
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
            var events = dbContext.Events.AsQueryable();
           
            // Filtrar por nombre del evento
            if (!string.IsNullOrEmpty(searchBy.EventName))
            {
                
                events = events.Where(e => e.Name.Contains(searchBy.EventName));
            }

            //Filtrar por ubicación del evento
            if (!string.IsNullOrEmpty(searchBy.Ubication))
            {
                
                events = events.Where(e => e.Ubicacion.Contains(searchBy.Ubication));
            }

            // Filtrar por fecha del evento
            if (searchBy.Date != null)
            {
                
                events = events.Where(e => e.Date.Date == searchBy.Date.Value.Date);
            }

            var result = await events.ToListAsync();
            return Ok(result);
        
        }

        [Authorize(Policy = "OrganizerPolicy")]
        // Crea un nuevo evento.
        // Permite a los usuarios crear un evento especificando el nombre, descripción, fecha, hora, ubicación y capacidad máxima de asistentes.
        // Parámetros:
        // - evento: Objeto que contiene los detalles del evento a crear.
        // Retorna:
        // - Respuesta HTTP indicando si se creó el evento exitosamente.
        [HttpPost("create")]
        public async Task<IActionResult> Create(Events evento)
        {

            //Consiguiendo id del usuario
            int organizerId = int.Parse((HttpContext.User.FindFirst("UserId")).Value);

            // Añade un evento a la base de datos

            var organizer = await organizersService.GetById(evento.OrganizersId);

            if (organizer is null)
                return BadRequest(new { message = $"El organizador {organizerId} no existe" });

            await eventsService.Create(evento);

            return Ok();
        }

        [Authorize(Policy = "OrganizerPolicy")]
        //Permitir al organizador del evento editar los detalles del evento en cualquier momento. 
        [HttpPut("udpate/{id}")]
        public async Task<ActionResult> Update(Events evento, int id)
        {

            //Consiguiendo id del usuario
            int userId = int.Parse((HttpContext.User.FindFirst("UserId")).Value);
            if (userId != evento.OrganizersId)
            {


            }
            if(evento.Id != id)
            {
                return BadRequest(new { message = $"Id de la url ({id}) no coincide con el del objeto({evento.Id})" });
            }


            if(userId != evento.OrganizersId)
            {
                return BadRequest(new { message = "You are not the organizer of this event" });
            }


            var eventToUpdate = await eventsService.GetById(evento.Id);

            if (eventToUpdate is not null)
            {
                await eventsService.Update(evento.Id, evento);
                return Accepted(new { message = $"Registro actualizado con exito" });
            }

            dbContext.Update(evento);
            await dbContext.SaveChangesAsync();
            return Ok();
        }
     
    }
}
