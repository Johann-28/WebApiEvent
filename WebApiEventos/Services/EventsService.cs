using Microsoft.AspNetCore.Mvc;
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
        //Los eventos se devuelven ordenados de mayor a menor

        public async Task<IEnumerable<EventsDto>> GetTop()
        {
            var events = await dbContext.Events
                .Include(a => a.Organizers)
                .Select(a => new EventsDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Description = a.Descripcion,
                    Date = a.Date.ToShortDateString(),
                    Hour = a.Date.ToShortTimeString(),
                    Ubication = a.Ubicacion,
                    Organizer = a.Organizers.Name,
                    Capacity = a.Capacidad 
                })
                .OrderByDescending(a => a.Capacity)
                .Take(5)
                .ToListAsync();

            return events;
        }

        // Obtiene una lista de todos los eventos en formato DTO.
        // Retorna una colección de objetos EventsDto que representan todos los eventos en formato DTO.
        public async Task<IEnumerable<EventsDto>> Get()
        {
            //la funcion Include hace un Join en base al identificador propio y la tabla Asistants
            var events = await dbContext.Events
                .Include(a => a.Organizers)
                .Select(a => new EventsDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Description = a.Descripcion,
                    Date = a.Date.ToShortDateString(),
                    Hour = a.Date.ToShortTimeString(),
                    Ubication = a.Ubicacion,
                    Organizer = a.Organizers.Name,
                    Capacity = a.Capacidad
                })
                .ToListAsync();

            return events;
        }


        // Obtiene un evento por su ID.
        // Parámetros:
        // - id: El ID del evento a buscar.
        // Retorna:
        // - Un objeto Events que representa el evento encontrado, o null si no se encontró.

        public async Task<Events?> GetById(int id)
        {
            return await dbContext.Events
        .Include(e => e.Assistants)
        .Include(e => e.Organizers)
        .FirstOrDefaultAsync(e => e.Id == id);
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

        // Busca eventos que coincidan con los criterios de búsqueda especificados.
        // Parámetros:
        //   searchBy: Objeto que contiene los criterios de búsqueda.
        // Retorna:
        //   Una colección de objetos Events que representan los eventos que coinciden con los criterios de búsqueda.
        public async Task<IEnumerable<Events>> SearchEvent(EventSearchDtoIn searchBy)
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
            return result;

        }

    }
}