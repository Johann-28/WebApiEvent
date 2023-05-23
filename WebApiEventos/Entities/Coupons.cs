namespace WebApiEventos.Entities
{
    public class Coupons
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Coupon { get; set; }
        public DateTime ExpireDate { get; set; }
        public int EventId { get; set; }
        public Events Events { get; set; }
 
    }
}
