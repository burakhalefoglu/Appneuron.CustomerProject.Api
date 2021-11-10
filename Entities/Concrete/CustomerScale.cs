using System.Collections.Generic;
using Core.Entities;

#nullable disable

namespace Entities.Concrete
{
    public class CustomerScale : IEntity
    {
        public short Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<Customer> Customers { get; set; }
    }
}