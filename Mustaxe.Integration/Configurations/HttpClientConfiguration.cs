using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mustaxe.Integration.Configurations
{
    [ExcludeFromCodeCoverage]
    public class HttpClientConfiguration
    {
        public string ClientName { get; set; }
        public string BaseURL { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public RetryPolicyConfiguration RetryPolicy { get; set; }
        public CiruitBreakerConfiguration CiruitBreaker { get; set; }
        public TimeoutPolicyConfiguration Timeout { get; set; }

        public HttpClientConfiguration() 
        {
            LoadConfiguration();
        }

        public void LoadConfiguration()
        {
            RetryPolicy = new RetryPolicyConfiguration(3, 500);
            CiruitBreaker = new CiruitBreakerConfiguration(3, 10000);
            Timeout = new TimeoutPolicyConfiguration(20000);
        }
    }

    [ExcludeFromCodeCoverage]
    public class RetryPolicyConfiguration 
    {
        public int Retries { get; set; }
        public int SleepDuration { get; set; }

        public RetryPolicyConfiguration(int retries, int sleepDuration)
        {
            Retries = retries;
            SleepDuration = sleepDuration;
        }
    }

    [ExcludeFromCodeCoverage]
    public class CiruitBreakerConfiguration
    {
        public int TimesBeforeOpen { get; set; }
        public int BreakDuration { get; set; }

        public CiruitBreakerConfiguration(int timesBeforeOpen, int breakDuration)
        {
            TimesBeforeOpen = timesBeforeOpen;
            BreakDuration = breakDuration;
        }
    }

    [ExcludeFromCodeCoverage]
    public class TimeoutPolicyConfiguration 
    {
        public int SleepDuration { get; set; }

        public TimeoutPolicyConfiguration(int sleepDuration)
        {
            SleepDuration = sleepDuration;
        }
    }
}
