using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiEventos.DTOs;
using WebApiEventos.Entities;
using WebApiEventos.Services;

namespace WebApiEventos.Controllers
{
   
    [ApiController]
    [Authorize(Policy = "OrganizerPolicy")]
    public class OrganizersController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly OrganizersService service;
        private readonly LoginService loginService;

        public OrganizersController(ApplicationDbContext dbContext, OrganizersService service, LoginService loginService)
        {
            this.dbContext = dbContext;
            this.service = service;
            this.loginService = loginService;
        }

        [HttpGet("get")]
        public async Task<IEnumerable<Organizers>> Get()
        {
            return await service.Get();
        }

        //Este metodo es el registro pero feo
        /* 
        [HttpPost("post")]
        public async Task<IActionResult> Create(Organizers organizer)
        {
            await service.Create(organizer);
            return Ok();

        }*/

      

    }
}
