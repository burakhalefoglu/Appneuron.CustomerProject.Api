using System;
using Core.Entities;

#nullable disable

namespace Entities.Concrete
{
    public class CustomerProject : IEntity
    {
        public bool Status = true;
        public long CustomerId { get; set; }
        public long VoteId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ProjectName { get; set; }
        public string ProjectBody { get; set; }
        public long Id { get; set; }
    }
}