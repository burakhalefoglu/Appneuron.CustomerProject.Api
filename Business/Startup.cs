using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Security.Claims;
using System.Security.Principal;
using Autofac;
using Business.Constants;
using Business.DependencyResolvers;
using Business.Fakes.DArch;
using Core.CrossCuttingConcerns.Caching;
using Core.CrossCuttingConcerns.Caching.Microsoft;
using Core.DependencyResolvers;
using Core.Extensions;
using Core.Utilities.ElasticSearch;
using Core.Utilities.IoC;
using Core.Utilities.MessageBrokers;
using Core.Utilities.MessageBrokers.Kafka;
using DataAccess.Abstract;
using DataAccess.Concrete.Cassandra;
using DataAccess.Concrete.Cassandra.Contexts;
using DataAccess.Concrete.Cassandra.Tables;
using DataAccess.Concrete.EntityFramework.Contexts;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Business
{
    public class BusinessStartup
    {
        protected readonly IHostEnvironment HostEnvironment;

        public BusinessStartup(IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            Configuration = configuration;
            HostEnvironment = hostEnvironment;
        }

        public IConfiguration Configuration { get; }

        /// <summary>
        ///     This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <remarks>
        ///     It is common to all configurations and must be called. Aspnet core does not call this method because there are
        ///     other methods.
        /// </remarks>
        /// <param name="services"></param>
        public virtual void ConfigureServices(IServiceCollection services)
        {
            Func<IServiceProvider, ClaimsPrincipal> getPrincipal = sp =>
                sp.GetService<IHttpContextAccessor>()?.HttpContext?.User ??
                new ClaimsPrincipal(new ClaimsIdentity(Messages.Unknown));

            services.AddScoped<IPrincipal>(getPrincipal);
            services.AddMemoryCache();

            services.AddDependencyResolvers(Configuration, new ICoreModule[]
            {
                new CoreModule()
            });

            services.AddSingleton<ConfigurationManager>();

            services.AddTransient<IElasticSearch, ElasticSearchManager>();
            services.AddTransient<IMessageBroker, KafkaMessageBroker>();
            services.AddSingleton<ICacheManager, MemoryCacheManager>();

            services.AddAutoMapper(typeof(ConfigurationManager));
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(typeof(BusinessStartup).GetTypeInfo().Assembly);

            ValidatorOptions.Global.DisplayNameResolver = (type, memberInfo, expression) =>
            {
                return memberInfo.GetCustomAttribute<DisplayAttribute>()?.GetName();
            };
        }

        /// <summary>
        ///     This method gets called by the Development
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureDevelopmentServices(IServiceCollection services)
        {
            ConfigureServices(services);
            
            services.AddTransient<IClientRepository>(x => new CassClientRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.Client));
            
            services.AddTransient<IGamePlatformRepository>(x => new CassGamePlatformRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.GamePlatform));
            
            services.AddTransient<ICustomerScaleRepository>(x => new CassCustomerScaleRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.CustomerScale));
            
            services.AddTransient<IVoteRepository>(x => new CassVoteRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.Vote));
            
            services.AddTransient<IIndustryRepository>(x => new CassIndustryRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.Industry));
            
            services.AddTransient<IDiscountRepository>(x => new CassDiscountRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.Discount));
            
            services.AddTransient<ICustomerDemographicRepository>(x => new CassCustomerDemographicRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.CustomerDemographic));
            
            services.AddTransient<IAppneuronProductRepository>(x => new CassAppneuronProductRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.AppneuronProduct));
            
            services.AddTransient<ICustomerRepository>(x => new CassCustomerRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.Customer));
            
            services.AddTransient<IInvoiceRepository>(x => new CassInvoiceRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.Invoice));
            
            services.AddTransient<ICustomerDiscountRepository>(x => new CassCustomerDiscountRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.CustomerDiscount));
            
            services.AddTransient<ICustomerProjectRepository>(x => new CassCustomerProjectRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.CustomerProject));
         
            services.AddTransient<ILogRepository>(x => new CassLogRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.Log));

            services.AddTransient<ICustomerProjectHasProductRepository>(x => new CassCustomerProjectHasProductRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.CustomerProjectHasProduct));
            
            services.AddTransient<IMessageBroker, KafkaMessageBroker>();
            services.AddDbContext<ProjectDbContext, DArchInMemory>(ServiceLifetime.Transient);
            services.AddSingleton<CassandraContextBase, Cassandracontext>();
        }

        /// <summary>
        ///     This method gets called by the Staging
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureStagingServices(IServiceCollection services)
        {
            ConfigureServices(services);
            services.AddTransient<IClientRepository>(x => new CassClientRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.Client));
            
            services.AddTransient<IGamePlatformRepository>(x => new CassGamePlatformRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.GamePlatform));
            
            services.AddTransient<ICustomerScaleRepository>(x => new CassCustomerScaleRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.CustomerScale));
            
            services.AddTransient<IVoteRepository>(x => new CassVoteRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.Vote));
            
            services.AddTransient<IIndustryRepository>(x => new CassIndustryRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.Industry));
            
            services.AddTransient<IDiscountRepository>(x => new CassDiscountRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.Discount));
            
            services.AddTransient<ICustomerDemographicRepository>(x => new CassCustomerDemographicRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.CustomerDemographic));
            
            services.AddTransient<IAppneuronProductRepository>(x => new CassAppneuronProductRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.AppneuronProduct));
            
            services.AddTransient<ICustomerRepository>(x => new CassCustomerRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.Customer));
            
            services.AddTransient<IInvoiceRepository>(x => new CassInvoiceRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.Invoice));
            
            services.AddTransient<ICustomerDiscountRepository>(x => new CassCustomerDiscountRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.CustomerDiscount));
            
            services.AddTransient<ICustomerProjectRepository>(x => new CassCustomerProjectRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.CustomerProject));
            
            services.AddTransient<ICustomerProjectHasProductRepository>(x => new CassCustomerProjectHasProductRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.CustomerProjectHasProduct));
         
            services.AddTransient<ILogRepository>(x => new CassLogRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.Log));
            

            services.AddTransient<IMessageBroker, KafkaMessageBroker>();

         //   services.AddDbContext<ProjectDbContext>();
            
            services.AddSingleton<CassandraContextBase, Cassandracontext>();        }

        /// <summary>
        ///     This method gets called by the Production
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureProductionServices(IServiceCollection services)
        {
            ConfigureServices(services);
            services.AddTransient<IClientRepository>(x => new CassClientRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.Client));
            
            services.AddTransient<IGamePlatformRepository>(x => new CassGamePlatformRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.GamePlatform));
            
            services.AddTransient<ICustomerScaleRepository>(x => new CassCustomerScaleRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.CustomerScale));
            
            services.AddTransient<IVoteRepository>(x => new CassVoteRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.Vote));
            
            services.AddTransient<IIndustryRepository>(x => new CassIndustryRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.Industry));
            
            services.AddTransient<IDiscountRepository>(x => new CassDiscountRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.Discount));
            
            services.AddTransient<ICustomerDemographicRepository>(x => new CassCustomerDemographicRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.CustomerDemographic));
            
            services.AddTransient<IAppneuronProductRepository>(x => new CassAppneuronProductRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.AppneuronProduct));
            
            services.AddTransient<ICustomerRepository>(x => new CassCustomerRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.Customer));
            
            services.AddTransient<IInvoiceRepository>(x => new CassInvoiceRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.Invoice));
            
            services.AddTransient<ICustomerDiscountRepository>(x => new CassCustomerDiscountRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.CustomerDiscount));
            
            services.AddTransient<ICustomerProjectRepository>(x => new CassCustomerProjectRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.CustomerProject));
            
            services.AddTransient<ICustomerProjectHasProductRepository>(x => new CassCustomerProjectHasProductRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.CustomerProjectHasProduct));
         
            services.AddTransient<ILogRepository>(x => new CassLogRepository(
                x.GetRequiredService<CassandraContextBase>(), CassandraTableQueries.Log));
            
            services.AddTransient<IMessageBroker, KafkaMessageBroker>();

           // services.AddDbContext<ProjectDbContext>();

            services.AddSingleton<CassandraContextBase, Cassandracontext>();
            
        }

        /// <summary>
        /// </summary>
        /// <param name="builder"></param>
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new AutofacBusinessModule(new ConfigurationManager(Configuration, HostEnvironment)));
        }
    }
}