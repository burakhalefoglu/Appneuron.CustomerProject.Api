using Core.DataAccess.EntityFramework;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework.Contexts;
using Entities.Concrete;

namespace DataAccess.Concrete.EntityFramework
{
    public class CustomerRepository : EfEntityRepositoryBase<Customer, ProjectDbContext>, ICustomerRepository
    {
        public CustomerRepository(ProjectDbContext context) : base(context)
        {
        }
    }
}