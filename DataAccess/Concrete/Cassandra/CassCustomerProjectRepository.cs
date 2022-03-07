using Cassandra.Mapping;
using Core.DataAccess.Cassandra;
using DataAccess.Abstract;
using DataAccess.Concrete.Cassandra.TableMappers;
using Entities.Concrete;

namespace DataAccess.Concrete.Cassandra;

public class CassCustomerProjectRepository: CassandraRepositoryBase<CustomerProject>, ICustomerProjectRepository
{
    public CassCustomerProjectRepository() : base(MappingConfiguration.Global.Define<CustomerProjectMapper>())
    {
    }
}