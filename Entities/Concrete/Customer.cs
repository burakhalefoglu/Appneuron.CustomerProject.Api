using Core.Entities;

#nullable disable

namespace Entities.Concrete
{
    public class Customer : IEntity
    {
        public Customer()
        {
            Status = true;
            CreatedAt = DateTimeOffset.Now;
        }
        public bool Status { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public long Id { get; set; }
    }
}