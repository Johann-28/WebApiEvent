using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApiEventos.DTOs;
using WebApiEventos.Entities;
using WebApiEventos.Services;

namespace WebApiEventos.Controllers
{
    // Controlador para gestionar las operaciones relacionadas con los cupones.
    [ApiController]
    [Route("api/[controller]")]
    public class CouponsController : ControllerBase
    {
   
        private readonly EventsService eventsService;
        private readonly CouponsService couponsService;

        public CouponsController(EventsService eventsService, CouponsService couponsService)
        {
            this.eventsService = eventsService;
            this.couponsService = couponsService;
        }

        // Obtiene todos los cupones.
        // Retorna:
        //   - Una colección de objetos CouponsDto que representan todos los cupones, incluyendo información adicional de eventos relacionados.
        [Authorize(Policy = "OrganizerPolicy")]
        [HttpGet("get")]
        public async Task<IEnumerable<CouponsDto>> Get()
        {
            return await couponsService.GetService();
        }

        // Crea un nuevo cupón.
        // Parámetros:
        //   - couponToAdd: Objeto CouponsDtoIn que contiene los detalles del cupón a crear.
        // Retorna:
        //   - Respuesta HTTP indicando si se creó el cupón exitosamente.
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

            await couponsService.CreateService(coupon);

            return Ok("Coupon created succesfully");
        }
    }
}
