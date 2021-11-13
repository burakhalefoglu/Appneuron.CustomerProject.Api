using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using Business.MessageBrokers;
using Business.MessageBrokers.Manager.GetClientCreationMessage;
using Business.MessageBrokers.Models;
using Core.Utilities.IoC;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WebAPI
{
    /// <summary>
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// </summary>
        /// <param name="args"></param>
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Server start");
            var result =  CreateHostBuilder(args).Build().RunAsync();
            var consumer = ConsumerAdapter();
            result.Wait();
            consumer.Wait();
        }

        /// <summary>
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(options => options.AddServerHeader = false);
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Trace);
                });
        }

        private static async Task ConsumerAdapter()
        {
            Console.WriteLine("Kafka Listening");
            var messageBroker = ServiceTool.ServiceProvider.GetService<IMessageBroker>();
            var clientCreationMessageService = ServiceTool.ServiceProvider.GetService<IGetClientCreationMessageService>();

            await messageBroker.GetMessageAsync<CreateClientMessageComamnd>("CreateClientMessageComamnd",
                "CreateClientConsumerGroup",
                clientCreationMessageService.GetClientCreationMessageQuery);


        }
    }
}