using Core.Entities;

#nullable disable

namespace Entities.Concrete
{
    public class CustomerDemographic : IEntity
    {
        public bool Status = true;
        public string CustomerDesc { get; set; }
        public long Id { get; set; }
    }
}