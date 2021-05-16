using Core.Entities;
using System.Collections.Generic;

#nullable disable

namespace Entities.Concrete
{
    public class CustomerDemographic : IEntity
    {
        public short Id { get; set; }
        public string CustomerDesc { get; set; }

        public virtual ICollection<Customer> Customers { get; set; }
    }
}