using Mustaxe.Integration.ViaCEP.Configurations;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mustaxe.Integration.Configurations
{
    [ExcludeFromCodeCoverage]
    public class IntegrationSettings
    {
        public ViaCEPSettings ViaCEP { get; set; }
    }
}
