using Microsoft.EntityFrameworkCore;
using WebApiEventos.Entities;

namespace WebApiEventos.Services
{
    // Servicio para manejar operaciones relacionadas con los usuarios.
    public class UsersService
    {
        private readonly ApplicationDbContext dbContext;

        // Inicializa una nueva instancia de la clase UsersService.
        // Parámetros:
        //   - dbContext: Contexto de la base de datos de la aplicación.
        public UsersService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
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

        // Crea un nuevo usuario.
        // Parámetros:
        //   - user: Objeto que contiene los detalles del usuario a crear.
        // Retorna:
        //   - El objeto Users que representa al usuario creado.
        public async Task<Users> Create(Users user)
        {
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();
            return user;
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



    }
}
