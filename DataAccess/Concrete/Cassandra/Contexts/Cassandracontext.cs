using Microsoft.Extensions.Configuration;

namespace DataAccess.Concrete.Cassandra.Contexts
{
    public class Cassandracontext: CassandraContextBase
    {
        public Cassandracontext(IConfiguration configuration) : base(configuration)
        {
        }
        
    }
}