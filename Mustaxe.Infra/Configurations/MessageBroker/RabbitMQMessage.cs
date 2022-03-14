using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mustaxe.Infra.Configurations.MessageBroker
{
    public class RabbitMQMessage : BrokerMessage
    {
        public string Queue { get; set; }
    }
}
