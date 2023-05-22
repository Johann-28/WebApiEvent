using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiEventos.DTOs;
using WebApiEventos.Entities;
using WebApiEventos.Services;

namespace WebApiEventos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly LoginService loginService;
        private IConfiguration config;
        public LoginController(ApplicationDbContext dbContext, LoginService loginService, IConfiguration config)
        {

            this.dbContext = dbContext;
            this.loginService = loginService;
            this.config = config;

        }

        [HttpPost("login/user")]
        public async Task<IActionResult> LoginUser(AccountDto userDto)
        {
            var user = await loginService.GetUser(userDto);

            if (user is null)
            {
                return BadRequest(new { message = "Invalid Credentials" });
            }

            string jwtToken = GenerateToken(user);

            return Ok(new { token = jwtToken });
        }

        // Crea un nuevo usuario.
        // Parámetros:
        //   - user: Objeto que contiene los detalles del usuario a crear.
        // Retorna:
        //   - Respuesta HTTP indicando si se creó el usuario exitosamente.
        [HttpPost("register/user")]
        public async Task<IActionResult> Create(Accounts account)
        {
            Users user = new Users();
            user.Name = account.Name;
            user.Email = account.Email;
            user.Password = account.Password;

            bool userRegistered = await dbContext.Users.AnyAsync(x => x.Email == account.Email);
            if (userRegistered)
            {
                return BadRequest(new { message = "User already registered" });
            }

            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            return Ok();
        }

        private string GenerateToken(Users user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim (ClaimTypes.Email, user.Email),
                new Claim("Email", user.Email),
                new Claim("UserId", user.Id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetSection("JWT:Key").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var securityToken = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: creds);

            string token = new JwtSecurityTokenHandler().WriteToken(securityToken);
            return token;
        }


        [HttpPost("login/organizer")]
        public async Task<IActionResult> LoginOrganizer(AccountDto userDto)
        {
            var organizer = await loginService.GetOrganizator(userDto);

            if (organizer is null)
            {
                return BadRequest(new { message = "Invalid Credentials" });
            }

            string jwtToken = GenerateOrganizerToken(organizer);

            return Ok(new { token = jwtToken });
        }


        private string GenerateOrganizerToken(Accounts organizer)
        {
            var claims = new[]
                 {
                new Claim(ClaimTypes.Name, organizer.Name),
                new Claim (ClaimTypes.Email, organizer.Email),
                new Claim("Email", organizer.Email),
                new Claim("UserId", organizer.Id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetSection("JWT:OrganizerKey").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var securityToken = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: creds);

            string token = new JwtSecurityTokenHandler().WriteToken(securityToken);
            return token;
        }

    }
}
