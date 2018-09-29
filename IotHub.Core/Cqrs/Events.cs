using System;
using System.Collections.Generic;
using System.Text;

namespace IotHub.Core.Cqrs
{
    public interface IEventPublisher
    {
        void Publish(IEvent e);
    }

    public interface IEventHandle<T> : ICqrsHandle where T : IEvent
    {
        void Handle(T e);
    }

    public interface IEvent
    {
        long Version { get; set; }
    }
}
