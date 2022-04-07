using Core.Entities;

namespace Entities.Concrete;

public class CustomerProjectHasProduct : IEntity
{
    public CustomerProjectHasProduct()
    {
        Status = true;
        CreatedAt = DateTimeOffset.Now;
    }

    public DateTimeOffset CreatedAt { get; set; }
    public long ProductId { get; set; }
    public long ProjectId { get; set; }
    public bool Status { get; set; }
    public long Id { get; set; }
}