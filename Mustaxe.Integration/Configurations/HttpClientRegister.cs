using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Mustaxe.Integration.ViaCEP.Configurations;
using Mustaxe.Integration.ViaCEP.Interfaces;
using Polly;
using Refit;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;

namespace Mustaxe.Integration.Configurations
{
    [ExcludeFromCodeCoverage]
    public static class HttpClientRegister
    {
        public static void RegisterHttpClients(this IServiceCollection services, IConfiguration configuration) 
        {
            var integrationSettings = services.BuildServiceProvider().GetService<IOptions<IntegrationSettings>>().Value;

            var configuracaoViaCEP = new ViaCEPConfiguration(integrationSettings);

            ConfigureHttpClient<IConsultaCEP>(services, configuracaoViaCEP);
        }

        private static void ConfigureHttpClient<TClient>(IServiceCollection services, HttpClientConfiguration httpClientConfiguration) 
            where TClient : class 
        {
            services.AddHttpClient<TClient>(httpClientConfiguration.ClientName, client =>
            {
                client.BaseAddress = new Uri(httpClientConfiguration.BaseURL);
            }).AddTypedClient(c =>
            {
                foreach (var header in httpClientConfiguration.Headers)
                    c.DefaultRequestHeaders.Add(header.Key, header.Value);

                return RestService.For<TClient>(c, new RefitSettings());
            }).AddPolicyHandler(ConfigurePolicies(httpClientConfiguration));
        }

        private static IAsyncPolicy<HttpResponseMessage> ConfigurePolicies(HttpClientConfiguration httpClientConfiguration) 
        {
            var policies = new List<IAsyncPolicy<HttpResponseMessage>>
            {
                PolicyFactory.CreateRetryPolicy(httpClientConfiguration.RetryPolicy.Retries, httpClientConfiguration.RetryPolicy.SleepDuration),
                PolicyFactory.CreateCircuitBreakerPolicy(httpClientConfiguration.CiruitBreaker.TimesBeforeOpen, httpClientConfiguration.CiruitBreaker.BreakDuration),
                PolicyFactory.CreateTimoutPolicy(httpClientConfiguration.Timeout.SleepDuration)
            };

            policies.RemoveAll(p => p == null);
            return Policy.WrapAsync(policies.ToArray());
        }
    }
}
