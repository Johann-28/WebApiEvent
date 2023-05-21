using Microsoft.AspNetCore.Mvc;
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

        [HttpPost("post")]
        public async Task<IActionResult> Create(Organizers organizer)
        {
            await service.Create(organizer);
            return Ok();

        }

    }
}
