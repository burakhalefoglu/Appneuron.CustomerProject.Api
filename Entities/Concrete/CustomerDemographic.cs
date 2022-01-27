using Core.Entities;

#nullable disable

namespace Entities.Concrete
{
    public class CustomerDemographic : DocumentDbEntity
    {
        public bool Status = true;
        public string CustomerDesc { get; set; }
    }
}