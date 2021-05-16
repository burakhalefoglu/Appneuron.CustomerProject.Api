using Core.Entities;

#nullable disable

namespace Entities.Concrete
{
    public class CustomerDiscount : IEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public short DiscountId { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Discount Discount { get; set; }
    }
}