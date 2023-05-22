using Microsoft.EntityFrameworkCore;
using WebApiEventos.Entities;

namespace WebApiEventos.Services
{
    // Servicio para manejar operaciones relacionadas con los usuarios.
    public class UsersService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly OrganizersService organizersService;

        // Inicializa una nueva instancia de la clase UsersService.
        // Parámetros:
        //   - dbContext: Contexto de la base de datos de la aplicación.
        public UsersService(ApplicationDbContext dbContext, OrganizersService organizersService)
        {
            this.dbContext = dbContext;
            this.organizersService = organizersService;
        }

        // Obtiene un usuario por su ID.
        // Parámetros:
        //   - id: ID del usuario a buscar.
        // Retorna:
        //   - El objeto Users que representa al usuario encontrado, o null si no se encontró.
        public async Task<Users?> GetById(int id)
        {
            return await dbContext.Users.FindAsync(id);
        }

    

        public async Task<String> IsValid(int userId, int eventId)
        {
            string isValid = "True";

            var user = await dbContext.Users.Include(u => u.Favorites).FirstOrDefaultAsync(u => u.Id == userId);
            var eventToAdd = await dbContext.Events.FindAsync(eventId);

            if (user is null)
            {
                return isValid = "User doesnt exists";
            }

            if (eventToAdd is null)
            {
                return isValid = "Event doesnts exists";
            }
            // Verificar si el evento ya existe en la lista de favoritos del usuario
            bool isAlreadyFavorite = user.Favorites.Any(e => e.Id == eventId);

            if (!isAlreadyFavorite)
            {
                return isValid;
            }
            return isValid = "Event Already Favorite";
        }

        public async Task<String> OrganizerValid(int userId, int organizerId)
        {
            string isValid = "True";

            var user = await dbContext.Users.Include(u => u.Organizations).FirstOrDefaultAsync(u => u.Id == userId);
            var organizer = await organizersService.GetById(organizerId);

            if (user is null)
            {
                return isValid = "User doesnt exists";
            }

            if (organizer is null)
            {
                return isValid = "Organizer doesnts exists";
            }
            // Verificar si el evento ya existe en la lista de favoritos del usuario
    
            bool isAlreadyFollowing = user.Organizations.Any(e => e.Id == organizerId);

            if (!isAlreadyFollowing)
            {
                return isValid;
            }
            return isValid = "Organizator already Followed";
        }

    }
}
