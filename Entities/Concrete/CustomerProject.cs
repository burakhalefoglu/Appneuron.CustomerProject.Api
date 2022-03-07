using System;
using Core.Entities;

#nullable disable

namespace Entities.Concrete
{
    public class CustomerProject : IEntity
    {
        public CustomerProject()
        {
            Status = true;
            CreatedAt = DateTimeOffset.Now;
        }
        public bool Status { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public long CustomerId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long Id { get; set; }
    }
}