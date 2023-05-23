using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebApiEventos.Entities
{
    public class Coupons
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Description field required")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Coupon field required")]
        public string Coupon { get; set; }
        [Required(ErrorMessage = "ExpireDate field required")]
        public DateTime ExpireDate { get; set; }
        [Required(ErrorMessage = "EventId field required")]
        public int EventId { get; set; }
        public Events Events { get; set; }
 
    }
}
