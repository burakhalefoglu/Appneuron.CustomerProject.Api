using System.Reflection;
using Autofac;
using Autofac.Extras.DynamicProxy;
using Castle.DynamicProxy;
using Core.Utilities.Interceptors;
using DataAccess.Abstract;
using DataAccess.Concrete.Cassandra;
using Module = Autofac.Module;

namespace Business.DependencyResolvers
{
    public class AutofacBusinessModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CassCustomerRepository>().As<ICustomerRepository>().SingleInstance();
            builder.RegisterType<CassCustomerProjectRepository>().As<ICustomerProjectRepository>().SingleInstance();
            builder.RegisterType<CassFeedbackRepository>().As<IFeedbackRepository>().SingleInstance();
            builder.RegisterType<CassLogRepository>().As<ILogRepository>().SingleInstance();
            builder.RegisterType<CassRateRepository>().As<IRateRepository>().SingleInstance();

            var assembly = Assembly.GetExecutingAssembly();

            builder.RegisterAssemblyTypes(assembly).AsImplementedInterfaces()
                .EnableInterfaceInterceptors(new ProxyGenerationOptions
                {
                    Selector = new AspectInterceptorSelector()
                }).SingleInstance().InstancePerDependency();
        }
    }
}
