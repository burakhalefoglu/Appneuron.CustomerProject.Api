using Core.Entities;

#nullable disable

namespace Entities.Concrete
{
    public class CustomerDiscount : IEntity
    {
        public CustomerDiscount()
        {
            Status = true;
            CreatedAt = DateTimeOffset.Now;
        }
        public bool Status { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public long CustomerId { get; set; }
        public long DiscountId { get; set; }
        public long Id { get; set; }
    }
}