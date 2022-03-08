using Cassandra.Mapping;
using Core.DataAccess.Cassandra.Configurations;
using Core.Utilities.IoC;
using Entities.Concrete;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccess.Concrete.Cassandra.TableMappers;

public class FeedbackMapper: Mappings
{
    public FeedbackMapper()
    {
        var configuration = ServiceTool.ServiceProvider.GetService<IConfiguration>();
        var cassandraConnectionSettings = 
            configuration.GetSection("CassandraConnectionSettings").Get<CassandraConnectionSettings>();
        For<Feedback>()
            .TableName("feedbacks")
            .KeyspaceName(cassandraConnectionSettings.Keyspace)
            .PartitionKey("id", "status")
            .ClusteringKey(new Tuple<string, SortOrder>("created_at", SortOrder.Ascending))
            .Column(u => u.Id, cm => cm.WithName("id").WithDbType(typeof(long)))
            .Column(u => u.CustomerId, cm => cm.WithName("customer_id").WithDbType(typeof(long)))
            .Column(u => u.Message, cm => cm.WithName("message").WithDbType(typeof(string)))
            .Column(u => u.CreatedAt, cm => cm.WithName("created_at").WithDbType(typeof(DateTimeOffset)))
            .Column(u => u.Status, cm => cm.WithName("status").WithDbType(typeof(bool)));

    }
}