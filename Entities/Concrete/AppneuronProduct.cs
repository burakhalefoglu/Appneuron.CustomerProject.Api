using Core.Entities;

#nullable disable

namespace Entities.Concrete
{
    public class AppneuronProduct : IEntity
    {
        public bool Status = true;
        public string ProductName { get; set; }
        public long Id { get; set; }
    }
}

