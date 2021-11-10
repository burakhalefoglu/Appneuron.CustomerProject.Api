using System;
using Core.Entities;

#nullable disable

namespace Entities.Concrete
{
    public class Client : IEntity
    {
        public long Id { get; set; }
        public string ClientId { get; set; }
        public long ProjectId { get; set; }
        public string ProjectKey { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsPaidClient { get; set; }

        public virtual CustomerProject CustomerProject { get; set; }
    }
}