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

        [HttpPost("register")]
        public async Task<IActionResult> Register(Accounts accountToRegister)
        {
            bool actuallyRegistered = await dbContext.OrganizersAccounts.AnyAsync(x => x.Email == accountToRegister.Email);

            if (actuallyRegistered)
            {
                return BadRequest(new { message = "Email already registered" });
            }
            await service.Register(accountToRegister);
            return Ok();

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(AccountDto organizerDto)
        {
            var organizer = await loginService.GetOrganizator(organizerDto);

            if (organizer is null)
            {
                return BadRequest(new { message = "Invalid Credentials" });
            }
            //generar token
            return Ok(new { token = "some value" });
        }

    }
}
