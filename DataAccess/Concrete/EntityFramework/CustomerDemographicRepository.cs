using Core.DataAccess.EntityFramework;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework.Contexts;
using Entities.Concrete;

namespace DataAccess.Concrete.EntityFramework
{
    public class CustomerDemographicRepository : EfEntityRepositoryBase<CustomerDemographic, ProjectDbContext>, ICustomerDemographicRepository
    {
        public CustomerDemographicRepository(ProjectDbContext context) : base(context)
        {
        }
    }
}