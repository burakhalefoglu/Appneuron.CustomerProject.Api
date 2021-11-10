using System;
using Core.Entities;

#nullable disable

namespace Entities.Concrete
{
    public class Invoice : IEntity
    {
        public long Id { get; set; }
        public string BillNo { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastPaymentTime { get; set; }
        public int UserId { get; set; }
        public short? DiscountId { get; set; }
        public int UnitPrice { get; set; }
        public bool IsItPaid { get; set; }

        public virtual Discount Discount { get; set; }
    }
}