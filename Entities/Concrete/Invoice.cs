using System;
using Core.Entities;

#nullable disable

namespace Entities.Concrete
{
    public class Invoice : IEntity
    {
        public bool Status = true;
        public string BillNo { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastPaymentTime { get; set; }
        public long UserId { get; set; }
        public long DiscountId { get; set; }
        public int UnitPrice { get; set; }
        public bool IsItPaid { get; set; }
        public long Id { get; set; }
    }
}