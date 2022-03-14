using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mustaxe.Infra.Configurations.MessageBroker
{
    public class RabbitMQSettings
    {
        public string BaseURL { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Port { get; set; }
    }
}
