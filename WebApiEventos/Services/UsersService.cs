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

    }
}
