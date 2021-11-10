using System.Collections.Generic;
using Core.Entities;

#nullable disable

namespace Entities.Concrete
{
    public class Discount : IEntity
    {
        public short Id { get; set; }
        public string DiscountName { get; set; }
        public short Percent { get; set; }

        public virtual ICollection<CustomerDiscount> CustomerDiscounts { get; set; }
        public virtual ICollection<Invoice> Invoices { get; set; }
    }
}