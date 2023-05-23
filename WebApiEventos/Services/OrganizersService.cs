using Microsoft.EntityFrameworkCore;
using WebApiEventos.Entities;

namespace WebApiEventos.Services
{
    // Servicio para gestionar las operaciones relacionadas con los organizadores.
    public class OrganizersService
    {
        private ApplicationDbContext dbContext;

        public OrganizersService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        // Obtiene todos los organizadores junto con sus eventos asociados.
        // Retorna:
        //   - Una colección de objetos Organizers que representan a los organizadores junto con sus eventos.
        public async Task<IEnumerable<Organizers>> Get()
        {
            return await dbContext.Organizers.Include(a => a.Events)
                .ToListAsync();
        }

        // Crea un nuevo organizador.
        // Parámetros:
        //   - organizer: Objeto Organizers que representa el nuevo organizador a crear.
        // Retorna:
        //   - Una tarea que representa la operación asincrónica de creación del organizador.
        public async Task Create(Organizers organizer)
        {
            await dbContext.Organizers.AddAsync(organizer);
            await dbContext.SaveChangesAsync();
            
        }

        // Obtiene un organizador por su ID.
        // Parámetros:
        //   - id: El ID del organizador a buscar.
        // Retorna:
        //   - El objeto Organizers correspondiente al ID especificado, o null si no se encuentra.
        public async Task<Organizers?> GetById(int id)
        {
            return await dbContext.Organizers.FindAsync(id);
        }

        // Registra una nueva cuenta de organizador.
        // Parámetros:
        //   - organizer: Objeto Accounts que representa la nueva cuenta de organizador a registrar.
        public async Task Register(Accounts organizer)
        {
            Organizers newAccount = new Organizers();
            newAccount.Name = organizer.Name;

          

            await Create(newAccount);

            await dbContext.OrganizersAccounts.AddAsync(organizer);
            await dbContext.SaveChangesAsync();
           


        }
    }
}
