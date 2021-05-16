using Core.Entities;
using System.Collections.Generic;

#nullable disable

namespace Entities.Concrete
{
    public class Industry : IEntity
    {
        public short Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Customer> Customers { get; set; }
    }
}