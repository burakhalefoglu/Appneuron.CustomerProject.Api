using Core.Entities;
using System.Collections.Generic;

#nullable disable

namespace Entities.Concrete
{
    public class Customer : IEntity
    {
        public int UserId { get; set; }
        public short CustomerScaleId { get; set; }
        public short? DemographicId { get; set; }
        public short IndustryId { get; set; }

        public virtual CustomerScale CustomerScaleNavigation { get; set; }
        public virtual CustomerDemographic Demographic { get; set; }
        public virtual Industry Industry { get; set; }
        public virtual ICollection<CustomerDiscount> CustomerDiscounts { get; set; }
        public virtual ICollection<CustomerProject> CustomerProjects { get; set; }
    }
}