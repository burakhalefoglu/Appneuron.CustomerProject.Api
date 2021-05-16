using Core.Entities.Concrete;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace DataAccess.Concrete.EntityFramework.Contexts
{
    /// <summary>
    /// Because this context is followed by migration for more than one provider
    /// works on PostGreSql db by default. If you want to pass sql
    /// When adding AddDbContext, use MsDbContext derived from it.
    /// </summary>
    public class ProjectDbContext : DbContext
    {
        protected readonly IConfiguration Configuration;

        /// <summary>
        /// in constructor we get IConfiguration, parallel to more than one db
        /// we can create migration.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="configuration"></param>
        public ProjectDbContext(DbContextOptions<ProjectDbContext> options, IConfiguration configuration)
                                                                                : base(options)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Let's also implement the general version.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="configuration"></param>
        protected ProjectDbContext(DbContextOptions options, IConfiguration configuration)
                                                                        : base(options)
        {
            Configuration = configuration;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                base.OnConfiguring(optionsBuilder.UseNpgsql(Configuration.GetConnectionString("DArchPgContext")).EnableSensitiveDataLogging());
            }
        }

        public DbSet<Log> Logs { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Translate> Translates { get; set; }
        public DbSet<CustomerProject> CustomerProjects { get; set; }
        public DbSet<CustomerDiscount> CustomerDiscounts { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<AppneuronProduct> AppneuronProducts { get; set; }
        public DbSet<CustomerDemographic> CustomerDemographics { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<Industry> Industries { get; set; }
        public DbSet<Vote> Votes { get; set; }
        public DbSet<CustomerScale> CustomerScales { get; set; }
        public DbSet<GamePlatform> GamePlatforms { get; set; }
        public DbSet<ProjectPlatform> ProjectPlatforms { get; set; }
    }
}