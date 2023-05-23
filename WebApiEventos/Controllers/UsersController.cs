﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using WebApiEventos.DTOs;
using WebApiEventos.DTOs.UserDto;
using WebApiEventos.Entities;
using WebApiEventos.Services;

namespace WebApiEventos.Controllers
{
    // Controlador para gestionar las operaciones relacionadas con los usuarios.
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "UserPolicy")]
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

        [HttpGet("upcomingEvents")]
        public async Task<ActionResult<List<EventsDto>>> UpComing()
        {

            //Consiguiendo id del usuario
            int userId = int.Parse((HttpContext.User.FindFirst("UserId")).Value);

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
        //   - Una lista de objetos Users que representan todos los usuarios registrados.

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

       

        [HttpGet("favorites/{eventId}")]
        public async Task<IActionResult> AddToFavorites( int eventId)
        {

            //Consiguiendo id del usuario
            int userId = int.Parse((HttpContext.User.FindFirst("UserId")).Value);

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

        [HttpGet("follow/{organizatorId}")]
        public async Task<IActionResult> FollowOrganizator(int organizatorId)
        {

            //Consiguiendo id del usuario
            int userId = int.Parse((HttpContext.User.FindFirst("UserId")).Value);

            //Determinando si los datos son correctos
            var isValid = await usersService.OrganizerValid(userId, organizatorId);


            if (isValid.Equals("True"))
            {
                var user = await usersService.GetById(userId);
                var organizer = await organizersService.GetById(organizatorId);
          
                user.Organizations.Add(organizer);
                await dbContext.SaveChangesAsync();
                return Ok(new { message = "followed successfully" });
            }
            return BadRequest(isValid);

        }

        [HttpGet("coupons")]
        public async Task<IEnumerable<CouponsDto>> Coupons()
        {
            //Consiguiendo id del usuario
            int userId = int.Parse((HttpContext.User.FindFirst("UserId")).Value);

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
        //Este metdo lo deje aqui comentado como demostracion de como es que accedo al id del usuario regisrado con solo su token
        /*
        [HttpGet("whoIam")]
        public  IActionResult WhatIsYourId()
        {
            var emailClaim = HttpContext.User.FindFirst("Email");
            
            if (emailClaim == null)
            {
                // El token no contiene la claim "UserId" o el usuario no está autenticado
                return Unauthorized();
            }

            var userIdClaim = HttpContext.User.FindFirst("UserId");
            if (userIdClaim == null)
            {
                // El token no contiene la claim "UserId" o el usuario no está autenticado
                return Unauthorized();
            }

            int id = int.Parse(userIdClaim.Value);
            string email = emailClaim.Value;

            return Ok(new {message = $"Su id es: {email} y su id es :{id}"});
        }*/

    }
}
