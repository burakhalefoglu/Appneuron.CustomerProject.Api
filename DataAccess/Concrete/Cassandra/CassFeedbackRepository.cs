using Cassandra.Mapping;
using Core.DataAccess.Cassandra;
using DataAccess.Abstract;
using DataAccess.Concrete.Cassandra.TableMappers;
using Entities.Concrete;

namespace DataAccess.Concrete.Cassandra;

public class CassFeedbackRepository : CassandraRepositoryBase<Feedback>, IFeedbackRepository
{
    public CassFeedbackRepository() : base(MappingConfiguration.Global.Define<FeedbackMapper>())
    {
    }
}