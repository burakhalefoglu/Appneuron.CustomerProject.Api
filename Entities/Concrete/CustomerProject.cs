using System;
using Core.Entities;

#nullable disable

namespace Entities.Concrete
{
    public class CustomerProject : DocumentDbEntity
    {
        public bool Status = true;
        public string CustomerId { get; set; }
        public string VoteId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string ProjectBody { get; set; }
    }
}