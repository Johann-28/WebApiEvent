using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiEventos.DTOs;
using WebApiEventos.Entities;
using WebApiEventos.Services;

namespace WebApiEventos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CouponsController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly EventsService eventsService;

        public CouponsController(ApplicationDbContext dbContext, EventsService eventsService)
        {
            this.dbContext = dbContext;
            this.eventsService = eventsService;
        }
       //[Authorize(Policy = "OrganizerPolicy")]
        [HttpGet("get")]
        public async Task<IEnumerable<CouponsDto>> Get()
        {
            return await dbContext.Coupons.Include(e => e.Events).Select(a => new CouponsDto
            {
                Description = a.Description,
                Coupon = a.Coupon,
                Date = a.ExpireDate.ToShortDateString(),
                Hour = a.ExpireDate.ToShortTimeString(),
                EventId = a.Events.Name
            }).ToListAsync();
        }

        [Authorize(Policy = "OrganizerPolicy")]
        [HttpPost("create")]
        public async Task<IActionResult> Create(CouponsDtoIn couponToAdd)
        {
            var evento = await eventsService.GetById(couponToAdd.EventId);

            if(evento is null)
            {
                return BadRequest(new { message = "The event doesnt exists" });
            }
            //Consiguiendo id del usuario
            int organizerId = int.Parse((HttpContext.User.FindFirst("UserId")).Value);

            if(organizerId != evento.OrganizersId) {
                return BadRequest(new {message = "Youre not organizer of this event"});
            }

            Coupons coupon = new Coupons();
            coupon.Description = couponToAdd.Description;
            coupon.Coupon = couponToAdd.Coupon;
            coupon.ExpireDate = couponToAdd.Date;
            coupon.EventId = couponToAdd.EventId;


            coupon.Events = evento;

            await dbContext.Coupons.AddAsync(coupon);
            await dbContext.SaveChangesAsync();

            return Ok("Coupon created succesfully");
        }
    }
}
