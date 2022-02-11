using Core.Entities;

#nullable disable

namespace Entities.Concrete
{
    public class CustomerDiscount : IEntity
    {
        public bool Status = true;
        public long UserId { get; set; }
        public long DiscountId { get; set; }
        public long Id { get; set; }
    }
}