using Mustaxe.MessageConsumer.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MessageConsumerServiceCollection
    {
        public static IServiceCollection AddMessageConsumerServices(this IServiceCollection services)
        {
            services.AddHostedService<RabbitMQConsumer>();

            return services;
        }
    }
}
