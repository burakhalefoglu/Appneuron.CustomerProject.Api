using Core.DataAccess;
using Core.DataAccess.Cassandra;
using Entities.Concrete;

namespace DataAccess.Abstract
{
    public interface IRateRepository : IRepository<Rate>
    {
    }
}