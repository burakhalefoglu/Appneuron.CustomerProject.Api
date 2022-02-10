using Core.DataAccess.Cassandra;
using DataAccess.Abstract;
using DataAccess.Concrete.Cassandra.Contexts;
using Entities.Concrete;

namespace DataAccess.Concrete.Cassandra
{
    public class CustomerScaleRepository : CassandraRepositoryBase<CustomerScale>,
        ICustomerScaleRepository
    {
        public CustomerScaleRepository(CassandraContexts cassandraContexts, string tableQuery) : base(
            cassandraContexts.CassandraConnectionSettings, tableQuery)
        {
        }

    }
}