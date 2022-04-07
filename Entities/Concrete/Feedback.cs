using Core.Entities;

namespace Entities.Concrete;

public class Feedback : IEntity
{
    public Feedback()
    {
        CreatedAt = DateTimeOffset.Now;
        Status = true;
    }

    public DateTimeOffset CreatedAt { get; set; }
    public string Message { get; set; }
    public long CustomerId { get; set; }
    public long Id { get; set; }
    public bool Status { get; set; }
}