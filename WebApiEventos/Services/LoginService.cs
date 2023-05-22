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

        public async Task<Accounts?> GetOrganizator(AccountDto organizer)
        {
            return await dbContext.OrganizersAccounts
                .SingleOrDefaultAsync(x => x.Email == organizer.Email && x.Password == organizer.Password);
        }

        public async Task<Users?> GetUser(AccountDto user)
        {
            return await dbContext.Users
                .SingleOrDefaultAsync(x => x.Email == user.Email && x.Password == user.Password);
        }


    }
}
