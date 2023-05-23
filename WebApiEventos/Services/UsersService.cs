using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiEventos.DTOs;
using WebApiEventos.Entities;

namespace WebApiEventos.Services
{
    // Servicio para manejar operaciones relacionadas con los usuarios.
    public class UsersService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly OrganizersService organizersService;
        private readonly EventsService eventsService;

        // Inicializa una nueva instancia de la clase UsersService.
        // Parámetros:
        //   - dbContext: Contexto de la base de datos de la aplicación.
        public UsersService(ApplicationDbContext dbContext, OrganizersService organizersService, EventsService eventsService)
        {
            this.dbContext = dbContext;
            this.organizersService = organizersService;
            this.eventsService = eventsService;
        }

        // Obtiene un usuario por su ID.
        // Parámetros:
        //   - id: ID del usuario a buscar.
        // Retorna:
        //   - El objeto Users que representa al usuario encontrado, o null si no se encontró.
        public async Task<Users?> GetById(int id)
        {
            return await dbContext.Users.FindAsync(id);
        }

        // Registra una nueva cuenta de usuario.
        // Parámetros:
        //   - user: Objeto Users que representa el nuevo usuario a registrar.
        public async Task Register(Users user)
        {
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

        }

        //   - Lista de eventos próximos en formato DTO (Data Transfer Object).
        public async Task<ActionResult<List<EventsDto>>> UpComing(int userId)
        {

            // Obtiene la fecha actual
            DateTime currentDate = DateTime.Now;

            // Calcula la fecha límite para los eventos próximos (7 días a partir de la fecha actual)
            DateTime deadlineDate = currentDate.AddDays(7);

            // Obtiene los eventos a los que el usuario asistirá y que están dentro del rango de fechas
            var upcomingEvents = await dbContext.Users
                .Include(u => u.Asistants)
                .ThenInclude(a => a.Event)
                .ThenInclude(o => o.Organizers)
                .Where(u => u.Id == userId)
                .SelectMany(u => u.Asistants.Where(a => a.Event.Date >= currentDate && a.Event.Date <= deadlineDate).Select(a => a.Event))
                .ToListAsync();

            // Mapea los eventos a la lista de DTOs
            List<EventsDto> eventsDtoList = upcomingEvents.Select(e => new EventsDto
            {
                Id = e.Id,
                Name = e.Name,
                Description = e.Descripcion,
                Date = e.Date.ToShortDateString(),
                Hour = e.Date.ToShortTimeString(),
                Ubication = e.Ubicacion,
                Organizer = e.Organizers.Name,
                Capacity = e.Capacidad
            }).ToList();

            return eventsDtoList;
        }

        // Obtiene todos los usuarios.
        // Retorna:
        //   - Una lista de objetos Users que representan a todos los usuarios en la base de datos.
        public async Task<ActionResult<List<Users>>> GetAll()
        {
            return await dbContext.Users
                .Include(a => a.Asistants)
                .Include(a => a.Comments)
                .Include(u => u.Favorites)
                .Include(a => a.Organizations)
                .Select(u => new Users
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    Password = u.Password,
                    Asistants = u.Asistants.Select(a => new Assistants
                    {
                        Id = a.Id,
                        EventId = a.EventId,
                        Event = new Events
                        {
                            Id = a.EventId,
                            Name = a.Event.Name,
                            Date = a.Event.Date,
                            Ubicacion = a.Event.Ubicacion,
                        }
                    }).ToList(),
                    Favorites = u.Favorites,
                    Organizations = u.Organizations.Select(o => new Organizers
                    {
                        Id = o.Id,
                        Name = o.Name
                    }).ToList(),
                    Comments = u.Comments.Select(c => new Comments
                    {
                        OrgnaizerId = c.OrgnaizerId,
                        Id = c.Id,
                        Comment = c.Comment,
                        Type = c.Type

                    }).ToList()
                })
                .ToListAsync();
        }

        // Agrega un evento a la lista de favoritos de un usuario.
        // Parámetros:
        //   - eventId: ID del evento a agregar a favoritos.
        //   - userId: ID del usuario.
        public async Task AddToFavoritesService(int eventId, int userId)
        {

                var user = await GetById(userId);
                var evento = await eventsService.GetById(eventId);

                user.Favorites.Add(evento);
                await dbContext.SaveChangesAsync();
          
        }

        // Sigue a un organizador agregándolo a la lista de organizaciones seguidas por un usuario.
        // Parámetros:
        //   - organizerId: ID del organizador a seguir.
        //   - userId: ID del usuario.
        public async Task FollowOrganizerService(int organizatorId, int userId)
        {

                var user = await GetById(userId);
                var organizer = await organizersService.GetById(organizatorId);

                user.Organizations.Add(organizer);
                await dbContext.SaveChangesAsync();
     
        }

        // Obtiene una lista de cupones válidos para un usuario específico.
        // Parámetros:
        //   - userId: ID del usuario.
        // Retorna:
        //   - Lista de cupones válidos en formato DTO (Data Transfer Object).
        public async Task<IEnumerable<CouponsDto>> CouponsService(int userId)
        {

            // Obtener el usuario actual
            var usuario = await dbContext.Users
                .Include(u => u.Asistants)
                .Include(u => u.Favorites)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (usuario == null)
            {
                return Enumerable.Empty<CouponsDto>();
            }

            // Obtener los IDs de los eventos en los que el usuario está registrado o tiene en favoritos
            var eventoIds = usuario.Asistants.Select(a => a.EventId)
                .Union(usuario.Favorites.Select(f => f.Id))
                .Distinct();

            // Obtener los cupones vigentes asociados a los eventos del usuario
            var cuponesVigentes = await dbContext.Coupons
                .Include(c => c.Events)
                .Where(c => eventoIds.Contains(c.EventId) && c.ExpireDate > DateTime.Now)
                .ToListAsync();


            // Proyectar los resultados en una lista de CouponsDto
            var couponsDto = cuponesVigentes.Select(c => new CouponsDto
            {
                Description = c.Description,
                Coupon = c.Coupon,
                Date = c.ExpireDate.ToShortDateString(),
                Hour = c.ExpireDate.ToShortTimeString(),
                EventId = c.Events.Name
            });

            return couponsDto;
        }

        // Verifica si un usuario puede agregar un evento a sus favoritos.
        // Parámetros:
        //   - userId: ID del usuario.
        //   - eventId: ID del evento.
        // Retorna:
        //   - Un mensaje indicando si el usuario puede agregar el evento a sus favoritos o el motivo por el cual no puede hacerlo.
        public async Task<String> IsValid(int userId, int eventId)
        {
            string isValid = "True";

            var user = await dbContext.Users.Include(u => u.Favorites).FirstOrDefaultAsync(u => u.Id == userId);
            var eventToAdd = await dbContext.Events.FindAsync(eventId);

            if (user is null)
            {
                return isValid = "User doesnt exists";
            }

            if (eventToAdd is null)
            {
                return isValid = "Event doesnts exists";
            }
            // Verificar si el evento ya existe en la lista de favoritos del usuario
            bool isAlreadyFavorite = user.Favorites.Any(e => e.Id == eventId);

            if (!isAlreadyFavorite)
            {
                return isValid;
            }
            return isValid = "Event Already Favorite";
        }

        // Verifica si un usuario puede seguir a un organizador.
        // Parámetros:
        //   - userId: ID del usuario.
        //   - organizerId: ID del organizador.
        // Retorna:
        //   - Un mensaje indicando si el usuario puede seguir al organizador o el motivo por el cual no puede hacerlo.

        public async Task<String> OrganizerValid(int userId, int organizerId)
        {
            string isValid = "True";

            var user = await dbContext.Users.Include(u => u.Organizations).FirstOrDefaultAsync(u => u.Id == userId);
            var organizer = await organizersService.GetById(organizerId);

            if (user is null)
            {
                return isValid = "User doesnt exists";
            }

            if (organizer is null)
            {
                return isValid = "Organizer doesnts exists";
            }
            // Verificar si el evento ya existe en la lista de favoritos del usuario
    
            bool isAlreadyFollowing = user.Organizations.Any(e => e.Id == organizerId);

            if (!isAlreadyFollowing)
            {
                return isValid;
            }
            return isValid = "Organizator already Followed";
        }

    }
}
