using Core.DataAccess.EntityFramework;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework.Contexts;
using Entities.Concrete;

namespace DataAccess.Concrete.EntityFramework
{
    public class CustomerProjectRepository : EfEntityRepositoryBase<CustomerProject, ProjectDbContext>, ICustomerProjectRepository
    {
        public CustomerProjectRepository(ProjectDbContext context) : base(context)
        {
        }
    }
}