﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiEventos.DTOs;
using WebApiEventos.Entities;
using WebApiEventos.Services;

namespace WebApiEventos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssistantsController : ControllerBase
    {
        private readonly EventsService eventsService;
        private readonly UsersService usersService;
        private readonly AssistantsService assistantsService;
        private readonly ApplicationDbContext dbContext;

        // Inicializa una nueva instancia de la clase AssistantsController.
        // Parámetros:
        //   - eventsService: Instancia del servicio EventsService.
        //   - usersService: Instancia del servicio UsersService.
        //   - assistantsService: Instancia del servicio AssistantsService.
        //   - dbContext: Contexto de la base de datos de la aplicación.
        public AssistantsController(EventsService eventsService, UsersService usersService, AssistantsService assistantsService, ApplicationDbContext dbContext)
        {
            this.eventsService = eventsService;
            this.usersService = usersService;
            this.assistantsService = assistantsService;
            this.dbContext = dbContext;
        }

        // Obtiene todos los asistentes registrados.
        // Retorna:
        //   - Una lista de objetos Assistants que representan a los asistentes registrados.
        [HttpGet("get")]
        public async Task<ActionResult<List<Assistants>>> Get()
        {
            return await dbContext.Asistants.Include(a => a.Event)
                .Include(a => a.User)
                .ToListAsync();
        }

        // Obtiene una lista de asistentes en formato DTO.
        // Retorna:
        //   - Una colección de objetos AssistantsDto que representan a los asistentes en formato DTO.
        [HttpGet("getdto")]
        public async Task<IEnumerable<AssistantsDto>> GetDto()
        {
            return await assistantsService.Get();
        }

        // Permite a los usuarios registrarse para un evento específico, manteniendo un registro de los asistentes y la cantidad de plazas disponibles.
        // Antes de registrar al usuario, se realiza una validación para verificar si el registro es válido.
        // Si la validación no pasa, se devuelve un BadRequest con un mensaje de error.
        // Después de registrar al usuario, se devuelve un Accepted indicando que el registro se actualizó exitosamente.
        // Parámetros:
        //   - userId: ID del usuario que se desea registrar como asistente.
        //   - eventId: ID del evento al que se desea registrar el usuario.
        // Retorna:
        //   - Un objeto IActionResult que indica el resultado de la operación.
        [HttpPost("register")]
        public async Task<IActionResult> Register(int userId, int eventId)
        {
            string result = await assistantsService.Validate(userId, eventId);
            if (result != "Valid")
            {
                return BadRequest(result);
            }

            var assistant = await assistantsService.Create(userId, eventId);

            return Accepted(new { message = "Registro actualizado con éxito" });
        }
    }
}