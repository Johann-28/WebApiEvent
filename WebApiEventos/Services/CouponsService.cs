using Microsoft.EntityFrameworkCore;
using WebApiEventos.DTOs;
using WebApiEventos.Entities;

namespace WebApiEventos.Services
{
    // Servicio para gestionar las operaciones relacionadas con los cupones.
    public class CouponsService
    {
        private readonly ApplicationDbContext dbContext;

        public CouponsService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        // Obtiene todos los cupones.
        // Retorna:
        //   - Una colección de objetos CouponsDto que representan todos los cupones, incluyendo información adicional de eventos relacionados.
        public async Task<IEnumerable<CouponsDto>> GetService()
        {
            return await dbContext.Coupons.Include(e => e.Events).Select(a => new CouponsDto
            {
                Description = a.Description,
                Coupon = a.Coupon,
                Date = a.ExpireDate.ToShortDateString(),
                Hour = a.ExpireDate.ToShortTimeString(),
                EventName = a.Events.Name
            }).ToListAsync();
        }

        // Crea un nuevo cupón.
        // Parámetros:
        //   - coupon: Objeto Coupons que representa el nuevo cupón a crear.
        // Retorna:
        //   - Respuesta HTTP indicando si se creó el cupón exitosamente.
        public async Task CreateService(Coupons coupon)
        {

            await dbContext.Coupons.AddAsync(coupon);
            await dbContext.SaveChangesAsync();
          
        }

    }
}
