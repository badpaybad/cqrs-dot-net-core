using System;
using System.Text;
using System.Threading;
using IotHub.Core.Cqrs;
using IotHub.Core.CqrsEngine;

namespace IotHub.Core.Cqrs.EventSourcingRepository
{
    public class EventPublisher : IEventPublisher
    {
        public void Publish(IEvent e)
        {
            CommandsAndEventsRegisterEngine.PushEvent(e);
        }
    }

}
