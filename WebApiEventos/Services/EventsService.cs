using Microsoft.EntityFrameworkCore;
using WebApiEventos.DTOs;
using WebApiEventos.Entities;

namespace WebApiEventos.Services
{
    public class EventsService
    {
        private ApplicationDbContext dbContext;

        public EventsService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        // Obtiene una lista de eventos en formato DTO.
        // Retorna:
        // - Una colección de objetos EventsDto que representan los eventos en formato DTO.

        public async Task<IEnumerable<EventsDto>> Get()
        {
            return await dbContext.Events.Select(a => new EventsDto
            {
                Name = a.Name,
                Description = a.Descripcion,
                Date = a.Date,
                Ubication = a.Ubicacion,
                Organizer = a.Organizers.Name
            }).ToListAsync();
        }

        // Obtiene un evento por su ID.
        // Parámetros:
        // - id: El ID del evento a buscar.
        // Retorna:
        // - Un objeto Events que representa el evento encontrado, o null si no se encontró.

        public async Task<Events?> GetById(int id)
        {
            return await dbContext.Events.FindAsync(id);
        }

        // Crea un nuevo evento.
        // Parámetros:
        // - newEvent: Objeto que contiene los detalles del evento a crear.
        // Retorna:
        // - El objeto Events que representa el evento creado.

        public async Task<Events> Create(Events newEvent)
        {
            dbContext.Events.Add(newEvent);
            await dbContext.SaveChangesAsync();
            return newEvent;
        }

        // Actualiza un evento existente.
        // Parámetros:
        // - id: El ID del evento a actualizar.
        // - eventToUpdate: Objeto que contiene los nuevos detalles del evento.
        public async Task Update(int id, Events eventToUpdate)
        {
            var existingEvent = await GetById(id);
            if(existingEvent is not null)
            {
                existingEvent.Name = eventToUpdate.Name;
                existingEvent.Descripcion = eventToUpdate.Descripcion;
                existingEvent.Date = eventToUpdate.Date; 
                existingEvent.Ubicacion = eventToUpdate.Ubicacion;
                existingEvent.Capacidad = eventToUpdate.Capacidad;

                await dbContext.SaveChangesAsync();
            }
        }

        // Elimina un evento por su ID.
        // Parámetros:
        // - id: El ID del evento a eliminar.
        public async Task Delete(int id)
        {
            var eventToDelete = await GetById(id);

            if(eventToDelete is not null)
            {
                dbContext.Events.Remove(eventToDelete);
                await dbContext.SaveChangesAsync();
            }
        }

    }
}
