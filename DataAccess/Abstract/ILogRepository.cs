using Core.DataAccess;
using Core.DataAccess.Cassandra;
using Core.Entities.Concrete;
using Entities.Concrete;

namespace DataAccess.Abstract
{
    public interface ILogRepository : IRepository<Log>, ICassandraRepository<Log>
    {
    }
}