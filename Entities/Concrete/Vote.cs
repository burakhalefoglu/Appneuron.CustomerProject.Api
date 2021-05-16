using Core.Entities;
using System.Collections.Generic;

#nullable disable

namespace Entities.Concrete
{
    public class Vote : IEntity
    {
        public short Id { get; set; }
        public string VoteName { get; set; }
        public short VoteValue { get; set; }

        public virtual ICollection<CustomerProject> CustomerProjects { get; set; }
    }
}