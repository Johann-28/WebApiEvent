using Microsoft.EntityFrameworkCore;
using WebApiEventos.DTOs;
using WebApiEventos.Entities;


namespace WebApiEventos.Services
{
    // Servicio para gestionar las operaciones relacionadas con los asistentes.
    public class AssistantsService
    {
        private ApplicationDbContext dbContext;
        private EventsService eventsService;
        private UsersService usersService;

        public async Task<IEnumerable<AssistantsDto>> Get()
        {
            return await dbContext.Asistants.Select(c => new AssistantsDto
            {
                Name = c.User.Name,     
                Event = c.Event.Name
            }).ToListAsync();
        }

        // Inicializa una nueva instancia de la clase AssistantsService.
        // Parámetros:
        //   - dbContext: Contexto de la base de datos de la aplicación.
        //   - eventsService: Instancia del servicio EventsService.
        //   - usersService: Instancia del servicio UsersService.
        public AssistantsService(ApplicationDbContext dbContext, EventsService eventsService, UsersService usersService)
        {
            this.dbContext = dbContext;
            this.eventsService = eventsService;
            this.usersService = usersService;
        }

        // Crea un nuevo asistente para un evento.
        // Parámetros:
        //   - userId: ID del usuario que se registrará como asistente.
        //   - eventId: ID del evento al que se registrará el usuario.
        // Retorna:
        //   - El objeto Assistants que representa el nuevo asistente creado.
        public async Task Create(int userId, int eventId)
        {
            Assistants assistant = new Assistants
            {
                UserId = userId,
                EventId = eventId
            };

            var eventToRegister = await eventsService.GetById(eventId);

            int newCapacity = eventToRegister.Capacidad - 1;
            eventToRegister.Capacidad = newCapacity;

            dbContext.Asistants.Add(assistant);
            await dbContext.SaveChangesAsync();

         
        }

        // Valida si un usuario puede registrarse como asistente a un evento.
        // Parámetros:
        //   - userId: ID del usuario que se desea validar.
        //   - eventId: ID del evento al que se desea validar el registro del usuario.
        // Retorna:
        //   - Un mensaje de validación indicando si el usuario puede registrarse o los motivos por los que no puede.
        public async Task<string> Validate(int userId, int eventId)
        {
            string result = "Valid";

            var eventToRegister = await eventsService.GetById(eventId);

            if (eventToRegister is null)
            {
                return result = "Event doesn't exist";
            }

            var userToRegister = await usersService.GetById(userId);

            if (userToRegister is null)
            {
                result = "User doesn't exist";
            }

            if (eventToRegister.Capacidad < 1)
            {
                result = "The event is already full";
            }

            bool isRegistered = await dbContext.Asistants
                .AnyAsync(a => a.UserId == userId && a.EventId == eventId);

            if (isRegistered)
            {
                result = "User is already registered for this event";
            }

            return result;
        }
    }
}
