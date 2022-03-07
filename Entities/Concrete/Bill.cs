using Core.Entities;

namespace Entities.Concrete
{
    public class Bill : IEntity
    {
        public Bill()
        {
            Status = true;
            CreatedAt = DateTimeOffset.Now;
        }
        public bool Status { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        
        public string BillNo { get; set; }
        public DateTime LastPaymentTime { get; set; }
        public long CustomerId { get; set; }
        public long DiscountId { get; set; }
        public int UnitPrice { get; set; }
        public bool IsItPaid { get; set; }
        public long Id { get; set; }
    }
}