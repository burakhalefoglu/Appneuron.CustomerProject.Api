using Core.Entities;

#nullable disable

namespace Entities.Concrete;

public class Product : IEntity
{
    public Product()
    {
        Status = true;
        CreatedAt = DateTimeOffset.Now;
    }

    public DateTimeOffset CreatedAt { get; set; }
    public string ProductName { get; set; }
    public bool Status { get; set; }
    public long Id { get; set; }
}