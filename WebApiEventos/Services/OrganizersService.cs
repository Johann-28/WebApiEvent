using Microsoft.EntityFrameworkCore;
using WebApiEventos.Entities;

namespace WebApiEventos.Services
{
    public class OrganizersService
    {
        private ApplicationDbContext dbContext;

        public OrganizersService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<Organizers>> Get()
        {
            return await dbContext.Organizers.Include(a => a.Events)
                .ToListAsync();
        }

        public async Task<Organizers> Create(Organizers organizer)
        {
            dbContext.Organizers.Add(organizer);
            await dbContext.SaveChangesAsync();
            return organizer;
        }

        public async Task<Organizers?> GetById(int id)
        {
            return await dbContext.Organizers.FindAsync(id);
        }
    }
}
