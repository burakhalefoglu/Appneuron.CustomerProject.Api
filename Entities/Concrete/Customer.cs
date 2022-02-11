using Core.Entities;

#nullable disable

namespace Entities.Concrete
{
    public class Customer : IEntity
    {
        public bool Status = true;
        public long CustomerScaleId { get; set; }
        
        public long DemographicId { get; set; }
        public long IndustryId { get; set; }
        public long Id { get; set; }
    }
}