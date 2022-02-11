using Core.Entities;

#nullable disable

namespace Entities.Concrete
{
    public class Discount : IEntity
    {
        public bool Status = true;
        public string DiscountName { get; set; }
        public short Percent { get; set; }
        public long Id { get; set; }
    }
}