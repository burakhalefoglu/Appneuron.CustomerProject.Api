using Core.DataAccess.MongoDb.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete.MongoDb.Context;
using Entities.Concrete;

namespace DataAccess.Concrete.Cassandra
{
    public class AppneuronProductRepository : MongoDbRepositoryBase<AppneuronProduct>, IAppneuronProductRepository
    {
        public AppneuronProductRepository(MongoDbContextBase mongoDbContext, string collectionName) : base(
            mongoDbContext.MongoConnectionSettings, collectionName)
        {
        }
    }
}