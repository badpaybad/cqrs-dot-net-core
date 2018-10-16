using IotHub.Core.Cqrs;
using System;

namespace IotHub.CommandsEvents.SampleDomainWithEventSourcing
{
    public class CreateSampleDomainWithEventSourcing : ICommand
    {
        public readonly Guid Id;
        public readonly string Version;

        public CreateSampleDomainWithEventSourcing(Guid id, string version)
        {
            Id = id;
            Version = version;
        }

        public Guid? PublishedCommandId { get; set; }
        public string TokenSession { get; set; }
    }

    public class ChangeVersionSampleDomainWithEventSourcing : ICommand
    {
        public readonly Guid Id;
        public readonly string Version;

        public ChangeVersionSampleDomainWithEventSourcing(Guid id, string version)
        {
            Id = id;
            Version = version;
        }

        public Guid? PublishedCommandId { get; set; }
        public string TokenSession { get; set; }
    }

    public class PublishSampleDomainWithEventSourcing : ICommand
    {
        public readonly Guid Id;

        public PublishSampleDomainWithEventSourcing(Guid id)
        {
            Id = id;
        }

        public Guid? PublishedCommandId { get; set; }
        public string TokenSession { get; set; }
    }

    public class UnpublishSampleDomainWithEventSourcing : ICommand
    {
        public readonly Guid Id;

        public UnpublishSampleDomainWithEventSourcing(Guid id)
        {
            Id = id;
        }

        public Guid? PublishedCommandId { get; set; }
        public string TokenSession { get; set; }
    }
}
