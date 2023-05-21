using Microsoft.AspNetCore.Mvc;
using WebApiEventos.Entities;

namespace WebApiEventos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        public EventsController()
        {
            
        }

        [HttpGet]
        public ActionResult<List<Events>> Get()
        {
            return new List<Events>()
            {
                new Events() {
                    Id = 1 , Name = "Luis Miguel" ,
                   Descripcion = "El sol de mexico" ,
                    Date = new DateTime(2022, 12 , 28),
                    Ubicacion = "FCFM",
                    Capacidad  = 500,
                    OrganizerId = 1,
                    Assistants = null,
                    Organizers = null
                }
            };
        }
    }
}
