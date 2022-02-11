using Core.Entities;

#nullable disable

namespace Entities.Concrete
{
    public class CustomerProjectHasProduct : IEntity
    {
        public long ProductId { get; set; }
        public long ProjectId { get; set; }
        public long Id { get; set; }
        public bool Status = true;

    }
}