using Microsoft.EntityFrameworkCore;
using WebApiEventos.DTOs;
using WebApiEventos.Entities;

namespace WebApiEventos.Services
{
    // Servicio para gestionar las operaciones relacionadas con los logins.
    public class LoginService
    {
        private readonly ApplicationDbContext dbContext;

        public LoginService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        // Obtiene el organizador correspondiente a las credenciales proporcionadas.
        // Parámetros:
        //   - organizer: Objeto AccountDto que contiene las credenciales del organizador.
        // Retorna:
        //   - El objeto Accounts correspondiente al organizador encontrado, o null si no se encuentra ningún organizador con las credenciales proporcionadas.
        public async Task<Accounts?> GetOrganizator(AccountDto organizer)
        {
            return await dbContext.OrganizersAccounts
                .SingleOrDefaultAsync(x => x.Email == organizer.Email && x.Password == organizer.Password);
        }

        // Obtiene el usuario correspondiente a las credenciales proporcionadas.
        // Parámetros:
        //   - user: Objeto AccountDto que contiene las credenciales del usuario.
        // Retorna:
        //   - El objeto Users correspondiente al usuario encontrado, o null si no se encuentra ningún usuario con las credenciales proporcionadas.
        public async Task<Users?> GetUser(AccountDto user)
        {
            return await dbContext.Users
                .SingleOrDefaultAsync(x => x.Email == user.Email && x.Password == user.Password);
        }

        // Verifica si ya está registrado un usuario con la dirección de correo electrónico proporcionada.
        // Parámetros:
        //   - account: Objeto Accounts que representa la cuenta a verificar.
        // Retorna:
        //   - Un valor booleano que indica si ya está registrado un usuario con la dirección de correo electrónico proporcionada.
        public async Task<bool> UserRegistered(Accounts account)
        {
            return await dbContext.Users.AnyAsync(x => x.Email == account.Email);
        }

        // Verifica si ya está registrado un organizador con la dirección de correo electrónico proporcionada.
        // Parámetros:
        //   - account: Objeto Accounts que representa la cuenta a verificar.
        // Retorna:
        //   - Un valor booleano que indica si ya está registrado un organizador con la dirección de correo electrónico proporcionada.
        public async Task<bool> OrganizerRegistered(Accounts account)
        {
            return await dbContext.OrganizersAccounts.AnyAsync(x => x.Email == account.Email);
        }



    }
}
