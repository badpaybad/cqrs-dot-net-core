using IotHub.Core.Cqrs;
using System;

namespace IotHub.CommandsEvents.SampleDomainWithEventSourcing
{
    public class SampleDomainWithEventSourcingCreated : IEvent
    {
        public readonly Guid Id;
        public readonly string SampleVersion;

        public SampleDomainWithEventSourcingCreated(Guid id, string sampleVersion)
        {
            this.Id = id;
            this.SampleVersion = sampleVersion;
        }

        public Guid PublishedEventId { get; set; }
      public  long Version { get; set; }
    }

    public class SampleDomainWithEventSourcingChangedVersion : IEvent
    {
        public readonly Guid Id;
        public readonly string SampleVersion;

        public SampleDomainWithEventSourcingChangedVersion(Guid id, string sampleVersion)
        {
            this.Id = id;
            this.SampleVersion = sampleVersion;
        }

        public Guid PublishedEventId { get; set; }
        public long Version { get; set; }
    }

    public class SampleDomainWithEventSourcingPublished : IEvent
    {
        public readonly Guid Id;

        public SampleDomainWithEventSourcingPublished(Guid id)
        {
            Id = id;
        }
        public Guid PublishedEventId { get; set; }
        public long Version { get; set; }
    }
    public class SampleDomainWithEventSourcingUnpublished : IEvent
    {
        public readonly Guid Id;

        public SampleDomainWithEventSourcingUnpublished(Guid id)
        {
            Id = id;
        }
        public Guid PublishedEventId { get; set; }
        public long Version { get; set; }
    }
}