using System;
using Core.Entities;

#nullable disable

namespace Entities.Concrete
{
    public class Client : DocumentDbEntity
    {
        public bool Status = true;
        public string ClientId { get; set; }
        public string ProjectId { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsPaidClient { get; set; }
    }
}