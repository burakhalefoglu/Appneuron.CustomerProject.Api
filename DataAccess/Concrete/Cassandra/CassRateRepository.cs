using Cassandra.Mapping;
using Core.DataAccess.Cassandra;
using DataAccess.Abstract;
using DataAccess.Concrete.Cassandra.TableMappers;
using Entities.Concrete;

namespace DataAccess.Concrete.Cassandra;

public class CassRateRepository : CassandraRepositoryBase<Rate>, IRateRepository
{
    public CassRateRepository() : base(MappingConfiguration.Global.Define<RateMapper>())
    {
    }
}
