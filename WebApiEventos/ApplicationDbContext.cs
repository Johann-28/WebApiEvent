using Microsoft.EntityFrameworkCore;
using WebApiEventos.Entities;

namespace WebApiEventos
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
            
        }
        public DbSet<Events> Events { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<Assistants> Asistants { get; set; }
        public DbSet<Comments> Comments { get; set; }
        public DbSet<Organizers> Organizers { get; set; }
        public DbSet<Accounts> OrganizersAccounts { get; set;}
        public DbSet<Coupons> Coupons { get; set; }   

    }
}
