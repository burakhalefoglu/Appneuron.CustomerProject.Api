using Core.Entities;

#nullable disable

namespace Entities.Concrete
{
    public class CustomerDiscount : DocumentDbEntity
    {
        public bool Status = true;
        public string UserId { get; set; }
        public string DiscountId { get; set; }
    }
}