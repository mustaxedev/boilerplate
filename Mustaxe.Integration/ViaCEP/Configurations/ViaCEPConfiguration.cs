using Microsoft.Extensions.Options;
using Mustaxe.Integration.Configurations;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mustaxe.Integration.ViaCEP.Configurations
{
    [ExcludeFromCodeCoverage]
    public class ViaCEPConfiguration : HttpClientConfiguration
    {
        private IntegrationSettings _settings;

        public ViaCEPConfiguration(IntegrationSettings settings) : base()
        {
            _settings = settings;

            LoadConfiguration();
        }

        public new void LoadConfiguration() 
        {
            ClientName = "ViaCEP";
            BaseURL = _settings.ViaCEP.BaseURL;
            Headers = new Dictionary<string, string>();
        }
    }
}
