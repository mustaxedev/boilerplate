using Polly;
using Polly.Extensions.Http;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Mustaxe.Integration.Configurations
{
    public class PolicyFactory
    {
        public PolicyFactory()
        {
        }

        public static IAsyncPolicy<HttpResponseMessage> CreateCircuitBreakerPolicy(int timesBeforeOpen, int breakDuration) 
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: timesBeforeOpen,
                    durationOfBreak: TimeSpan.FromMilliseconds(breakDuration)
                );
        }

        public static IAsyncPolicy<HttpResponseMessage> CreateRetryPolicy(int retries, int sleepDuration) 
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(
                    retryCount: retries,
                    sleepDurationProvider: attemp => TimeSpan.FromMilliseconds(sleepDuration)
                );
        }

        public static IAsyncPolicy<HttpResponseMessage> CreateTimoutPolicy(int timeout) 
        {
            return Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromMilliseconds(timeout));
        }
    }
}
