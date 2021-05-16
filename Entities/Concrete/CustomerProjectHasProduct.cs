using Core.Entities;

#nullable disable

namespace Entities.Concrete
{
    public class CustomerProjectHasProduct : IEntity
    {
        public long Id { get; set; }
        public short ProductId { get; set; }
        public long ProjectId { get; set; }

        public virtual AppneuronProduct Product { get; set; }
        public virtual CustomerProject Project { get; set; }
    }
}