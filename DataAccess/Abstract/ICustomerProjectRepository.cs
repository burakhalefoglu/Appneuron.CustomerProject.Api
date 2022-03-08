using Core.DataAccess;
using Core.DataAccess.Cassandra;
using Entities.Concrete;

namespace DataAccess.Abstract
{
    public interface ICustomerProjectRepository : IRepository<CustomerProject>
    {
    }
}