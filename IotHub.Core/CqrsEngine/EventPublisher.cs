using System;
using System.Text;
using System.Threading;
using IotHub.Core.Cqrs;

namespace IotHub.Core.CqrsEngine
{
    public class EventPublisher : IEventPublisher
    {
        public void Publish(IEvent e)
        {
            CommandsAndEventsRegisterEngine.Push(e);
        }
    }

}
