using System.Collections.Generic;
using Core.Entities;

#nullable disable

namespace Entities.Concrete
{
    public class AppneuronProduct : IEntity
    {
        public short Id { get; set; }
        public string ProductName { get; set; }

        public virtual ICollection<CustomerProjectHasProduct> CustomerProjectHasProducts { get; set; }
    }
}