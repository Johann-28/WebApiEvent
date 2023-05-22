using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiEventos.DTOs;
using WebApiEventos.DTOs.UserDto;
using WebApiEventos.Entities;
using WebApiEventos.Services;

namespace WebApiEventos.Controllers
{
    // Controlador para gestionar las operaciones relacionadas con los usuarios.
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly UsersService usersService;
        private readonly EventsService eventsService;
        private readonly OrganizersService organizersService;
        private readonly LoginService loginService;
        private IConfiguration config;

        // Inicializa una nueva instancia de la clase UsersController.
        // Parámetros:
        //   - dbContext: Contexto de la base de datos de la aplicación.
        public UsersController(ApplicationDbContext dbContext, UsersService usersService, EventsService eventsService, OrganizersService organizersService, LoginService loginService, IConfiguration config)
        {
            this.dbContext = dbContext;
            this.usersService = usersService;
            this.eventsService = eventsService;
            this.organizersService = organizersService;
            this.loginService = loginService;
            this.config = config;
        }

        [HttpGet("getdto")]
        public async Task<ActionResult<List<UsersDto>>> GetDto()
        {
            return await dbContext.Users
                .Include(a => a.Asistants)
                .Include(c => c.Comments)
                .Include(f => f.Favorites)
                .Include(o => o.Organizations)
                .Select(u => new UsersDto
                {
                    Name = u.Name,
                    Email = u.Email,
                    Assistants = u.Asistants.Select(a => new UsersEventsDto
                    {

                        EventName = a.Event.Name,
                        Date = a.Event.Date.ToShortDateString(),
                        Ubication = a.Event.Ubicacion,
                        Hour = a.Event.Date.ToShortTimeString()

                    }).ToList(),
                    Comments = u.Comments.Select(c => new UsersCommentsDto
                    {

                        Comment = c.Comment,
                        Type = c.Type == 1 ? "Pregunta" : "Comentario",
                        OrganizerName = c.Organizers.Name

                    }).ToList(),
                    Favorites = u.Favorites.Select(f => new UsersEventsDto
                    {
                        EventName = f.Name,
                        Date = f.Date.ToShortDateString(),
                        Ubication = f.Ubicacion,
                        Hour = f.Date.ToShortTimeString()
                    }).ToList(),
                    Organizers = u.Organizations.Select(o => new UsersOrganizersFollowed
                    {
                        OrganizerName = o.Name
                    }).ToList()
                })
                .ToListAsync();
        }


        // Obtiene todos los usuarios.
        // Retorna:
        //   - Una lista de objetos Users que representan todos los usuarios registrados.
        [Authorize(Policy ="User")]
        [HttpGet("get")]
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
                        Id=a.Id,
                        EventId =a.EventId,
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
                    Comments = u.Comments.Select(c=> new Comments
                    {
                        Id = c.Id,
                        Comment = c.Comment,
                        Type = c.Type
   
                    }).ToList()
                })
                .ToListAsync();
        }

       

        [HttpGet("{userId}/favorites/{eventId}")]
        public async Task<IActionResult> AddToFavorites(int userId, int eventId)
        {
            var isValid = await usersService.IsValid(userId, eventId);
          
            
            if (isValid.Equals("True"))
            {
                var user = await usersService.GetById(userId);
                var evento = await eventsService.GetById(eventId);
                
                user.Favorites.Add(evento);
                await dbContext.SaveChangesAsync();
                return Ok();
            }
            return BadRequest(isValid);
          
        }

        [HttpGet("{userId}/follow/{organizatorId}")]
        public async Task<IActionResult> FollowOrganizator(int userId, int organizatorId)
        {
            var isValid = await usersService.OrganizerValid(userId, organizatorId);


            if (isValid.Equals("True"))
            {
                var user = await usersService.GetById(userId);
                var organizer = await organizersService.GetById(organizatorId);
          
                user.Organizations.Add(organizer);
                await dbContext.SaveChangesAsync();
                return Ok();
            }
            return BadRequest(isValid);

        }


       
    }
}
