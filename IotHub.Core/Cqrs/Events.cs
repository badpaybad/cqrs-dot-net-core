using System;
using System.Collections.Generic;
using System.Text;

namespace IotHub.Core.Cqrs
{    
    public interface IEventHandle<T> : ICqrsHandle where T : IEvent
    {
        void Handle(T e);
    }

    public interface IEvent
    {
        Guid PublishedEventId { get; set; }
        long Version { get; set; }
    }
}
