using System;
using System.Text;
using System.Threading;
using IotHub.Core.Cqrs;
using IotHub.Core.CqrsEngine;

namespace IotHub.Core.Cqrs.EventSourcingRepository
{
    public interface IEventPublisher
    {
        void Publish(IEvent e);
    }

    public class EventPublisher : IEventPublisher
    {
        public void Publish(IEvent e)
        {
            if (e.PublishedEventId == Guid.Empty)
            {
                e.PublishedEventId = Guid.NewGuid();
            }
            CommandsAndEventsRegisterEngine.PushEvent(e);
        }
    }

}
