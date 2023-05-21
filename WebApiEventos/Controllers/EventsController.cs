using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiEventos.Entities;
using WebApiEventos.Services;
using WebApiEventos.DTOs;

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

        //Regresa todos los registros de la tabla eventos,
        //NECESITARAS AUTORIZACION
        [HttpGet("getall")]
        public async Task<ActionResult<List<Events>>> GetAll()
        {

            //la funcion Include hace un Join en base al identificador propio y la tabla Asistants
            return await dbContext.Events
                .Include(a => a.Assistants).Include(a => a.Organizers)
                .ToListAsync();
        }

        // Obtiene una lista de eventos en formato DTO (Data Transfer Object).
        // Permite mostrar una lista de todos los eventos creados, proporcionando información básica como el nombre, fecha y ubicación.
        // Esto permite a los usuarios explorar los eventos y decidir a cuál desean asistir.

        [HttpGet("getdto")]
        public async Task<IEnumerable<EventsDto>> GetDTOs()
        {
            return await eventsService.Get();
        }


        //Regresa el evento con la id solicitada
        [HttpGet("get/{id}")]
        public async Task<ActionResult<Events>> GetById(int id)
        {
            var evento = await eventsService.GetById(id);

            if (evento is null)
                return BadRequest("El evento no existe");
            return evento;

        }

        // Crea un nuevo evento.
        // Permite a los usuarios crear un evento especificando el nombre, descripción, fecha, hora, ubicación y capacidad máxima de asistentes.
        // Parámetros:
        // - evento: Objeto que contiene los detalles del evento a crear.
        // Retorna:
        // - Respuesta HTTP indicando si se creó el evento exitosamente.
        [HttpPost("create")]
        public async Task<IActionResult> Create(Events evento)
        {
            // Añade un evento a la base de datos

            var organizer = await organizersService.GetById(evento.OrganizersId);

            if (organizer is null)
                return BadRequest(new { message = $"El organizador {evento.OrganizersId} no existe" });

            await eventsService.Create(evento);

            return Ok();
        }

        //Permitir al organizador del evento editar los detalles del evento en cualquier momento.
        //FALTA AÑADIR AUTORIZACION 
        [HttpPut("udpate/{id}")]
        public async Task<ActionResult> Update(int id, Events evento)
        {
            if (evento.Id != id)
            {
                return BadRequest(new { message = $"El ID({id}) de la URL no coincide con el ID({evento.Id}) del cuerpo de la solicitud." });
            }

            var eventToUpdate = await eventsService.GetById(id);

            if (eventToUpdate is not null)
            {
                await eventsService.Update(id, evento);
                return Accepted(new { message = $"Registro actualizado con exito" });
            }

            dbContext.Update(evento);
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
                
            var eventToDelete = await eventsService.GetById(id);

            if (eventToDelete is not null)
            {
                await eventsService.Delete(id);
                return Accepted(new { message = $"Registro borrado con exito" });
            }
            else
            {
                return BadRequest("No existe el evento");
            }

        }


    }
}
