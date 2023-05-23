namespace WebApiEventos.DTOs
{
    public class CouponsDtoIn
    {
        public string Description { get; set; }
        public string Coupon { get; set; }
        public DateTime Date { get; set; }
        public int EventId { get; set; }
    }
}
