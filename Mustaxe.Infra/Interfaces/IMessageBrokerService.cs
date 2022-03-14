using Mustaxe.Infra.Configurations.MessageBroker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mustaxe.Infra.Interfaces
{
    public interface IMessageBrokerService<T> where T : BrokerMessage
    {
        void SendMessage(T message);
    }
}
