using Core.Entities;

#nullable disable

namespace Entities.Concrete
{
    public class Discount : IEntity
    {
        public Discount()
        {
            Status = true;
            CreatedAt = DateTimeOffset.Now;
        }
        public bool Status { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public string DiscountName { get; set; }
        public short Percent { get; set; }
        public long Id { get; set; }
    }
}