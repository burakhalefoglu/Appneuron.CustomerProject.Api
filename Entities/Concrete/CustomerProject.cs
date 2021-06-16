using System;
using Core.Entities;
using System.Collections.Generic;

#nullable disable

namespace Entities.Concrete
{
    public class CustomerProject : IEntity
    {
        public long Id { get; set; }
        public int CustomerId { get; set; }
        public short? VoteId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ProjectKey { get; set; }
        public string ProjectName { get; set; }
        public string ProjectBody { get; set; }
        public bool? Statuse { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Vote Vote { get; set; }
        public virtual ICollection<CustomerProjectHasProduct> CustomerProjectHasProducts { get; set; }
        public virtual ICollection<ProjectPlatform> ProjectPlatforms { get; set; }
        public virtual ICollection<Client> Clients { get; set; }

    }
}
