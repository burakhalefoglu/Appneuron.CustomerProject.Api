using System;
using Core.Entities;

#nullable disable

namespace Entities.Concrete
{
    public class Client : IEntity
    {
        public bool Status = true;
        public long ClientId { get; set; }
        public long ProjectId { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsPaidClient { get; set; }
        public long Id { get; set; }
    }
}