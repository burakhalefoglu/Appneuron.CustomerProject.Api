using Core.DataAccess.Cassandra;
using DataAccess.Abstract;
using DataAccess.Concrete.Cassandra.Contexts;
using Entities.Concrete;

namespace DataAccess.Concrete.Cassandra
{
    public class CustomerDemographicRepository : CassandraRepositoryBase<CustomerDemographic>,
        ICustomerDemographicRepository
    {
        public CustomerDemographicRepository(CassandraContexts cassandraContexts, string tableQuery) : base(
            cassandraContexts.CassandraConnectionSettings, tableQuery)
        {
        }
    }
}