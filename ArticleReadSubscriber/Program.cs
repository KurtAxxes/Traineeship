using ArticleReadSubscriber.Handlers;
using AxxesTimes.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using System;
using System.Threading.Tasks;

namespace ArticleReadSubscriber
{
    class Program
    {
        private static ServiceProvider serviceProvider;

        static async Task Main()
        {
            Console.Title = "ArticleReadSubscriber";

            IConfiguration Configuration = new ConfigurationBuilder()
                                                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                                                    .Build();

            serviceProvider = AddRegistrations(Configuration);

            await StartListeningAsync();
        }

        private static ServiceProvider AddRegistrations(IConfiguration Configuration)
        {
            //setup our DI
            return new ServiceCollection()
                .AddSingleton(Configuration)
                .AddScoped<IArticlesRepository, ArticlesRepository>()
                .BuildServiceProvider();
        }

        private static async Task StartListeningAsync()
        {
            // configure nservicebus
            var endpointConfiguration = new EndpointConfiguration("ArticleReadSubscriber");
            endpointConfiguration.RegisterComponents(
                registration: components =>
                {
                    components.ConfigureComponent(
                        componentFactory: builder =>
                        {
                            return new ArticleReadHandler(serviceProvider.GetService<IArticlesRepository>());
                        },
                        dependencyLifecycle: DependencyLifecycle.InstancePerUnitOfWork);
                });
            var transport = endpointConfiguration.UseTransport<LearningTransport>();

            // start listening for incoming messages
            var endpointInstance = await Endpoint.Start(endpointConfiguration)
                                                 .ConfigureAwait(false);

            Console.WriteLine("Press <any key> to exit.");
            Console.ReadKey();

            // stop listening
            await endpointInstance.Stop()
                                  .ConfigureAwait(false);
        }
    }
}
