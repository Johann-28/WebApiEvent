using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiEventos.DTOs;
using WebApiEventos.Entities;
using WebApiEventos.Services;

namespace WebApiEventos.Controllers
{
    // Controlador para gestionar las operaciones relacionadas con los logeos.
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
      
        private readonly LoginService loginService;
        private readonly OrganizersService organizersService;
        private readonly UsersService usersService;
        private readonly IConfiguration config;
        public LoginController( LoginService loginService, IConfiguration config, OrganizersService organizersService, UsersService usersService)
        {

            this.loginService = loginService;
            this.config = config;
            this.organizersService = organizersService;
            this.usersService = usersService;
        }

        // Inicia sesión para un usuario.
        // Parámetros:
        //   - userDto: Objeto AccountDto que contiene las credenciales del usuario.
        // Retorna:
        //   - Respuesta HTTP indicando si el inicio de sesión fue exitoso junto con el token JWT.
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

            bool userRegistered = await loginService.UserRegistered(account);
            if (userRegistered)
            {
                return BadRequest(new { message = "User already registered" });
            }

            await usersService.Register(user);

            return Ok();
        }

        // Genera un token JWT para un usuario.
        // Parámetros:
        //   - user: Objeto Users que representa al usuario para el cual se generará el token.
        // Retorna:
        //   - El token JWT generado como una cadena de caracteres.
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

        // Inicia sesión para un organizador.
        // Parámetros:
        //   - userDto: Objeto AccountDto que contiene las credenciales del organizador.
        // Retorna:
        //   - Respuesta HTTP indicando si el inicio de sesión fue exitoso junto con el token JWT.
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

        // Crea un nuevo organizador.
        // Parámetros:
        //   - accountToRegister: Objeto Accounts que contiene los detalles del organizador a crear.
        // Retorna:
        //   - Respuesta HTTP indicando si se creó el organizador exitosamente.
        [HttpPost("register/organizer")]
        public async Task<IActionResult> Register(Accounts accountToRegister)
        {
            bool actuallyRegistered = await loginService.OrganizerRegistered(accountToRegister);

            if (actuallyRegistered)
            {
                return BadRequest(new { message = "Email already registered" });
            }
            await organizersService.Register(accountToRegister);
            return Ok();

        }


        // Genera un token JWT para un organizador.
        // Parámetros:
        //   - organizer: Objeto Accounts que representa al organizador para el cual se generará el token.
        // Retorna:
        //   - El token JWT generado como una cadena de caracteres.
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
