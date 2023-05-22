using Microsoft.AspNetCore.Mvc;
using WebApiEventos.DTOs;
using WebApiEventos.Services;

namespace WebApiEventos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly LoginService loginService;

        public LoginController(LoginService loginService)
        {
            this.loginService = loginService;
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Login(OrganizerAccountDto organizerDto)
        {
            var organizer = await loginService.GetOrganizator(organizerDto);

            if(organizer is null)
            {
                return BadRequest(new { message = "Invalid Credentials" });
            }
            //generar token
            return Ok(new { token = "some value" });
        }
    }

}
