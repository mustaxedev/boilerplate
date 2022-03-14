using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Mustaxe.API.Configurations;
using System.Diagnostics.CodeAnalysis;

namespace Mustaxe.API.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class MappingRegister
    {
        public static void RegisterMapping(this IServiceCollection services) 
        {
            var mapperConfiguration = new MapperConfiguration(options =>
            {
                options.AddProfile(new MappingProfile());
            });
            IMapper mapper = mapperConfiguration.CreateMapper();
            services.AddSingleton(mapper);
        }
    }
}
