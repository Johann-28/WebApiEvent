using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiEventos.Entities;
using WebApiEventos.Services;

namespace WebApiEventos.Controllers
{
    // Controlador para gestionar las operaciones relacionadas con los usuarios.
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly UsersService usersService;
        private readonly EventsService eventsService;

        // Inicializa una nueva instancia de la clase UsersController.
        // Parámetros:
        //   - dbContext: Contexto de la base de datos de la aplicación.
        public UsersController(ApplicationDbContext dbContext, UsersService usersService, EventsService eventsService)
        {
            this.dbContext = dbContext;
            this.usersService = usersService;
            this.eventsService = eventsService;
        }

        // Obtiene todos los usuarios.
        // Retorna:
        //   - Una lista de objetos Users que representan todos los usuarios registrados.
        [HttpGet("get")]
        public async Task<ActionResult<List<Users>>> GetAll()
        {
            return await dbContext.Users
                .Include(a => a.Asistants)
                .Include(a => a.Comments).Include(u => u.Favorites)
                .ToListAsync();
        }

        // Crea un nuevo usuario.
        // Parámetros:
        //   - user: Objeto que contiene los detalles del usuario a crear.
        // Retorna:
        //   - Respuesta HTTP indicando si se creó el usuario exitosamente.
        [HttpPost("post")]
        public async Task<IActionResult> Create(Users user)
        {
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            return Ok();
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
    }
}
