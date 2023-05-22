using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiEventos.Entities;
using WebApiEventos.Services;

namespace WebApiEventos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrganizersController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly OrganizersService service;

        public OrganizersController(ApplicationDbContext dbContext, OrganizersService service)
        {
            this.dbContext = dbContext;
            this.service = service;
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

    }
}
