# Api para gestion de eventos

<p style ="text-align :justify">
 La API se realizo con un modelo servicio-controlador pues es una forma debio a que el servicio tiene acceso a la base de datos a traves de una variable privada y el controlador tiene acceso al servicio a traves de una variable privada, por lo que la encapsulacion de datos es su punto fuerte aumentando al seguridad de al misma.
 A continuacion se mostrarán las partes del código responsables de cada punto, mostrando la funcion del servicio y controlador respectivo. Para más detalles en el codigo fuente se encuentra el código documentado de manera detallada para un mejor entendimiento.

</p>

#
A continuacion se muestra un diagrama entidad relacion de la base de datos de la API:
<br>

![Diagrama-DER](https://github.com/Johann-28/WebApiEvent/tree/master/VisualMaterial/Diagrama-DER.png)

<div style ="text-align :justify">

 <details><summary> <b>Permitir a los organizadores crear eventos, especificando el nombre, la descripción, la fecha y la hora, la ubicación y la capacidad máxima de asistentes. </b> </summary> 
<p>

### Servicio



```csharp
public class EventsService{

    private readonly ApplicationDbContext dbContext();

    public EventsService(ApplicationDbContext dbContext){
        this.dbContext = dbContext;
    }

    public async Task<Events> Create(Events newEvent)
        {
            dbContext.Events.Add(newEvent);
            await dbContext.SaveChangesAsync();
            return newEvent;
        }
}
```
### Controlador
```csharp

    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {

        private readonly EventsService eventsService;
            public EventsController(EventsService eventsService)
            {

                this.eventsService = eventsService;
            }

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
}
```

</p>
</details>



<br>

<details><summary><b>Mostrar una lista de todos los eventos creados, con información básica como el nombre, la fecha y la ubicación, para que los usuarios puedan explorarlos y decidir a cuál asistir.</b></summary>
<p>

### Servicio
```csharp
public class EventsService{

    private readonly ApplicationDbContext dbContext();

    public EventsService(ApplicationDbContext dbContext){
        this.dbContext = dbContext;
    }

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
}

```
### Controlador
```csharp
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {

        private readonly EventsService eventsService;
            public EventsController(EventsService eventsService)
            {

                this.eventsService = eventsService;
            }

        [HttpGet("events")]
        public async Task<IEnumerable<EventsDto>> GetAll()
        {
            return await eventsService.Get();
        }
```

</p>
</details>
<br>

<details><summary><b>Permitir a los usuarios registrarse para un evento en particular, manteniendo un registro de los asistentes y la cantidad de plazas disponibles.</b></summary>
<p>

### Servicio
```csharp
public class AssistantsService{

        private ApplicationDbContext dbContext;
        private EventsService eventsService;
        private UsersService usersService;

        public AssistantsService(ApplicationDbContext dbContext, EventsService eventsService, UsersService usersService)
        {
            this.dbContext = dbContext;
            this.eventsService = eventsService;
            this.usersService = usersService;
        }

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

```
### Controlador
```csharp
   // Controlador para gestionar las operaciones relacionadas con los usuarios.
    [ApiController]
    [Route("api/[controller]")]
    public class AssistantsController : ControllerBase
    {
        private readonly EventsService eventsService;
        private readonly AssistantsService assistantsService;

       
        public AssistantsController(EventsService eventsService, AssistantsService assistantsService)
        {
            this.eventsService = eventsService;
            this.assistantsService = assistantsService;
 
        }

        [HttpGet("events")]
        public async Task<IEnumerable<EventsDto>> GetAll()
        {
            return await eventsService.Get();
        }

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

            await assistantsService.Create(userId, eventId);

            return Accepted(new { message = "Registration successful" });
        }

    }
```
</p>
</details>
<br>

<details><summary><b>Mostrar un formulario para que los usuarios puedan enviar preguntas o comentarios al organizador del evento. </b></summary>
<p>

### Servicio
```csharp

    public class CommentsService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly OrganizersService organizersService;

        public CommentsService(ApplicationDbContext dbContext, OrganizersService organizersService)
        {
            this.dbContext = dbContext;
            this.organizersService = organizersService;
        }

         public async Task PostService(Comments comentario, int userId)
        {

            // Agrega el comentario a la base de datos.
            dbContext.Comments.Add(comentario);
            await dbContext.SaveChangesAsync();

        }
    }

```

### Controller
```csharp

    [ApiController]
    [Route("api/[controller]")]
    public class CommentsController : ControllerBase
    {
     
        private readonly CommentsService commentsService;
        private readonly OrganizersService organizationizersService;
        private readonly UsersService usersService;
        public CommentsController(CommentsService commentsService,  OrganizersService organizationizersService, UsersService usersService)
        {
       
            this.commentsService = commentsService;
            this.organizationizersService = organizationizersService;
            this.usersService = usersService;
        }

        [Authorize(Policy = "UserPolicy")]
        [HttpPost("post")]
        public async Task<IActionResult> Post(Comments comentario)
        {

            //Consiguiendo id del usuario
            int userId = int.Parse((HttpContext.User.FindFirst("UserId")).Value);

            // Verifica si el tipo de comentario es válido (1: Pregunta, 2: Comentario).
            if (comentario.Type != 1 && comentario.Type != 2)
            {
                return BadRequest(new { message = "Enter 1 for a question or 2 for a comment" });
            }

            var user = await usersService.GetById(userId);
            if(user is null)
            {
                return BadRequest(new { message = "User doesnt exists" });
            }

            var organizer = await organizationizersService.GetById(comentario.OrgnaizerId);
            if(organizer is null)
            {
                return BadRequest(new { message = $"Organizer {comentario.OrgnaizerId} doesnt exists" });
            }

            comentario.Organizers = organizer;
            comentario.User = user;

            // Agrega el comentario a la base de datos.
            await commentsService.PostService(comentario, userId);

            // Retorna una respuesta exitosa junto con un mensaje.
            return Ok(new { message = "Post created successfully" });
        }
    }

```
</p>
</details>
<br>
<details><summary><b>Permitir al organizador del evento editar los detalles del evento en cualquier momento.</b></summary>
<p>

### Servicio
```csharp
    public class EventsService
    {
        private ApplicationDbContext dbContext;

        public EventsService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        // Actualiza un evento existente.
        // Parámetros:
        // - id: El ID del evento a actualizar.
        // - eventToUpdate: Objeto que contiene los nuevos detalles del evento.
        public async Task UpdateService(int id, Events eventToUpdate)
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
    }
```


### Controlador

```csharp
   
[ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {

        private readonly EventsService eventsService;
        public EventsController(EventsService eventsService)
        {

            this.eventsService = eventsService;
        }

        // Permitir al organizador del evento editar los detalles del evento en 
        // cualquier momento. 
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

```

</p>
</details>

<br>
<details><summary><b>Enviar recordatorios automáticos a los usuarios registrados para un evento, unos días antes de la fecha programada. </b></summary>
<p>

### Logica de servicio!
```csharp
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

    }
```

### Controlador

```csharp
   
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "UserPolicy")]
    public class UsersController : ControllerBase
    {
        
        private readonly UsersService usersService;
       
        public UsersController(UsersService usersService)
        {
            
            this.usersService = usersService;
    
        }

        [HttpGet("upcomingEvents")]
        public async Task<ActionResult<List<EventsDto>>> UpComing()
        {

            //Consiguiendo id del usuario
            int userId = int.Parse((HttpContext.User.FindFirst("UserId")).Value);

            var eventsDtoList = await usersService.UpComing(userId);

            return eventsDtoList;

        }

    }
```

</p>
</details>
<br>

<details><summary><b>Permitir a los usuarios buscar eventos por nombre, fecha o ubicación.</b></summary>
<p>

### Servicio
```csharp
   
    public class EventsService
        {
            private ApplicationDbContext dbContext;

            public EventsService(ApplicationDbContext dbContext)
            {
                this.dbContext = dbContext;
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

```
### Controlador
```csharp
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly EventsService eventsService;
        public EventsController(EventsService eventsService)
        {

            this.eventsService = eventsService;
        }


        //Recibe un objeto con la fecha, nombre y ubicacion y se filtran
        [HttpPost("search")]
        public async Task<IActionResult> Search(EventSearchDtoIn searchBy)
        {
            var result = await eventsService.SearchEvent(searchBy);
            return Ok(result);
        
        }
    }
```

</p>
</details>
<br>

<details><summary><b>Permitir a los usuarios seguir a un organizador en particular para recibir actualizaciones de sus próximos eventos. </b></summary>
<p>


### Service
```csharp
   public class UsersService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly OrganizersService organizersService;
        private readonly EventsService eventsService;


        public UsersService(ApplicationDbContext dbContext, OrganizersService organizersService, EventsService eventsService)
        {
            this.dbContext = dbContext;
            this.organizersService = organizersService;
            this.eventsService = eventsService;
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

    }
```

### Logica de servicio!
```csharp
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "UserPolicy")]
    public class UsersController : ControllerBase
    {
        
        private readonly UsersService usersService;
       
        public UsersController(UsersService usersService)
        {
            
            this.usersService = usersService;
    
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

    }
```

</p>
</details>
<br>

<details><summary><b>Mostrar una lista de eventos populares o destacados para que los usuarios puedan descubrir nuevos eventos de su interés.</b></summary>
<p>

### Servicio
```csharp
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

    }
```
### Controlador
```csharp
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {

       
        private readonly EventsService eventsService;
        public EventsController(EventsService eventsService)
        {

            this.eventsService = eventsService;
        }

        // Obtiene una lista de eventos en formato DTO (Data Transfer Object).
        // Permite mostrar una lista de todos los eventos creados, proporcionando información básica como el nombre, fecha y ubicación.
        // Esto permite a los usuarios explorar los eventos y decidir a cuál desean asistir.
        [HttpGet("trending")]
        public async Task<IEnumerable<EventsDto>> Trending()
        {
            return await eventsService.GetTop();
        }
    }
```

</p>
</details>
<br>

<details><summary><b>Permitir a los usuarios marcar eventos como favoritos para poder acceder fácilmente a ellos más tarde. <b></summary>
<p>

### Servicio
```csharp
   public class UsersService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly OrganizersService organizersService;
        private readonly EventsService eventsService;

        public UsersService(ApplicationDbContext dbContext, OrganizersService organizersService, EventsService eventsService)
        {
            this.dbContext = dbContext;
            this.organizersService = organizersService;
          
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

    }
```
### Controlador
```csharp
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "UserPolicy")]
    public class UsersController : ControllerBase
    {
        
        private readonly UsersService usersService;
       
        public UsersController(UsersService usersService)
        {
            
            this.usersService = usersService;
    
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

    }
```
</p>
</details>
<br>

<details><summary><b>Mantener un registro de los eventos a los que un usuario ha asistido en el pasado, junto con su historial de registro y asistencia. <b></summary>
<p>

### Servicio
```csharp
   
    public class AssistantsService
    {
        private ApplicationDbContext dbContext;
        private EventsService eventsService;
        private UsersService usersService;

        public AssistantsService(ApplicationDbContext dbContext, EventsService eventsService, UsersService usersService)
        {
            this.dbContext = dbContext;
            this.eventsService = eventsService;
            this.usersService = usersService;
        }

        public async Task<IEnumerable<AssistantsDto>> Get()
        {
            return await dbContext.Asistants.Select(c => new AssistantsDto
            {
                Name = c.User.Name,     
                Event = c.Event.Name
            }).ToListAsync();
        }

    }

```
### Controlador
```csharp
    [ApiController]
    [Route("api/[controller]")]
    public class AssistantsController : ControllerBase
    {
        private readonly EventsService eventsService;
        private readonly AssistantsService assistantsService;

        public AssistantsController(EventsService eventsService, AssistantsService assistantsService)
        {
            this.eventsService = eventsService;
            this.assistantsService = assistantsService;
 
        }
        // Obtiene una lista de asistentes en formato DTO.
        // Retorna:
        //   - Una colección de objetos AssistantsDto que representan a los asistentes en formato DTO.
        [HttpGet("get")]
        public async Task<IEnumerable<AssistantsDto>> GetDto()
        {
            return await assistantsService.Get();
        }
        
    }
```
</p>
</details>
<br>

<details><summary><b>Permitir a los organizadores crear descuentos o promociones para un evento en particular, y enviar códigos promocionales a los usuarios registrados.</b></summary>
<p>

### Servicio para hacer el cupon
```csharp
   public class CouponsService
    {
        private readonly ApplicationDbContext dbContext;

        public CouponsService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        // Crea un nuevo cupón.
        // Parámetros:
        //   - coupon: Objeto Coupons que representa el nuevo cupón a crear.
        // Retorna:
        //   - Respuesta HTTP indicando si se creó el cupón exitosamente.
        public async Task CreateService(Coupons coupon)
        {

            await dbContext.Coupons.AddAsync(coupon);
            await dbContext.SaveChangesAsync();
          
        }

    }
```
### Controlador para hacer el cupon
```csharp
    [ApiController]
    [Route("api/[controller]")]
    public class CouponsController : ControllerBase
    {
   
        private readonly EventsService eventsService;
        private readonly CouponsService couponsService;

        public CouponsController(EventsService eventsService, CouponsService couponsService)
        {
            this.eventsService = eventsService;
            this.couponsService = couponsService;
        }

        [Authorize(Policy = "OrganizerPolicy")]
        [HttpPost("create")]
        public async Task<IActionResult> Create(CouponsDtoIn couponToAdd)
        {
            var evento = await eventsService.GetById(couponToAdd.EventId);

            if(evento is null)
            {
                return BadRequest(new { message = "The event doesnt exists" });
            }
            //Consiguiendo id del usuario
            int organizerId = int.Parse((HttpContext.User.FindFirst("UserId")).Value);

            if(organizerId != evento.OrganizersId) {
                return BadRequest(new {message = "Youre not organizer of this event"});
            }

            Coupons coupon = new Coupons();
            coupon.Description = couponToAdd.Description;
            coupon.Coupon = couponToAdd.Coupon;
            coupon.ExpireDate = couponToAdd.Date;
            coupon.EventId = couponToAdd.EventId;


            coupon.Events = evento;

            await couponsService.CreateService(coupon);

            return Ok("Coupon created succesfully");
        }
    }
```
### Servicio para mostrar cupones
```csharp
public class UsersService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly OrganizersService organizersService;
        private readonly EventsService eventsService;

        public UsersService(ApplicationDbContext dbContext, OrganizersService organizersService, EventsService eventsService)
        {
            this.dbContext = dbContext;
            this.organizersService = organizersService;
            this.eventsService = eventsService;
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
                EventName = c.Events.Name
            });

            return couponsDto;
        }
    }
```
### Controlador para mostrar cupones
```csharp
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "UserPolicy")]
    public class UsersController : ControllerBase
    {
        
        private readonly UsersService usersService;
       

        public UsersController(UsersService usersService)
        {
            
            this.usersService = usersService;
    
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
```

</p>
</details>
</div>

