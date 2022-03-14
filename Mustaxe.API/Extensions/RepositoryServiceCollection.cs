using Mustaxe.Domain.Entities;
using Mustaxe.Persistence.Repository;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RepositoryServiceCollection
    {
        public static IServiceCollection AddRepositoryServices(this IServiceCollection services) 
        {
            services.AddScoped<IRepository<ConsultaCEP>, Repository<ConsultaCEP>>();

            return services;
        }
    }
}
