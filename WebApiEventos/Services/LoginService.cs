using Microsoft.EntityFrameworkCore;
using WebApiEventos.DTOs;
using WebApiEventos.Entities;

namespace WebApiEventos.Services
{
    public class LoginService
    {
        private readonly ApplicationDbContext dbContext;

        public LoginService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<OrganizerAccounts?> GetOrganizator(OrganizerAccountDto organizer)
        {
            return await dbContext.OrganizersAccounts
                .SingleOrDefaultAsync(x => x.Email == organizer.Email && x.Password == organizer.Password);
        }

    }
}
