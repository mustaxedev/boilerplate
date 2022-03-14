using Mustaxe.Infra.Configurations.MessageBroker;
using Mustaxe.Infra.Interfaces;
using Mustaxe.Infra.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class InfraServiceCollection
    {
        public static IServiceCollection AddInfraServices(this IServiceCollection services) 
        {
            services.AddScoped<IMessageBrokerService<RabbitMQMessage>, RabbitMQService>();

            return services;
        }
    }
}
