using System;
using Core.Entities;

#nullable disable

namespace Entities.Concrete
{
    public class Invoice : DocumentDbEntity
    {
        public bool Status = true;
        public string BillNo { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastPaymentTime { get; set; }
        public string UserId { get; set; }
        public string DiscountId { get; set; }
        public int UnitPrice { get; set; }
        public bool IsItPaid { get; set; }
    }
}