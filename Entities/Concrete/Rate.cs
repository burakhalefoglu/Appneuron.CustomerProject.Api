using Core.Entities;

namespace Entities.Concrete
{
    public class Rate : IEntity
    {
        public Rate()
        {
            Status = true;
            CreatedAt = DateTimeOffset.Now;
        }
        public bool Status { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public short Value { get; set; }
        public long CustomerId { get; set; }
        public long Id { get; set; }
    }
}