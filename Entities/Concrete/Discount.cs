using Core.Entities;

#nullable disable

namespace Entities.Concrete
{
    public class Discount : DocumentDbEntity
    {
        public bool Status = true;
        public string DiscountName { get; set; }
        public short Percent { get; set; }
    }
}