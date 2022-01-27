using Core.Entities;

#nullable disable

namespace Entities.Concrete
{
    public class Customer : DocumentDbEntity
    {
        public bool Status = true;
        public string CustomerScaleId { get; set; }
        public string DemographicId { get; set; }
        public string IndustryId { get; set; }
    }
}