using IotHub.Core.Cqrs;
using System;
using System.Collections.Generic;
using System.Text;

namespace IotHub.CommandsEvents.SampleDomain
{
    public class SampleEventCreated : IEvent
    {
        public Guid PublishedEventId { get; set; }
        public long Version { get; set; }

        public string SampleVersion { get; set; }
    }
}
