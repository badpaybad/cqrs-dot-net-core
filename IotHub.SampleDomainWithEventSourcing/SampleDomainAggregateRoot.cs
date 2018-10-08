using IotHub.CommandsEvents.SampleDomainWithEventSourcing;
using IotHub.Core.Cqrs;
using System;

namespace IotHub.SampleDomainWithEventSourcing
{
    public class SampleDomainAggregateRoot : AggregateRoot
    {
        public override Guid Id { get; set; }

        public SampleDomainAggregateRoot() { }

        string _version;
        bool _published;

        void Apply(SampleDomainWithEventSourcingCreated e)
        {
            Id = e.Id;
            _version = e.SampleVersion;
        }
        void Apply(SampleDomainWithEventSourcingChangedVersion e)
        {
            Id = e.Id;
            _version = e.SampleVersion;
        }
        void Apply(SampleDomainWithEventSourcingPublished e)
        {
            _published = true;
        }

        void Apply(SampleDomainWithEventSourcingUnpublished e)
        {
            _published = false;
        }

        public void Create(Guid id, string version)
        {
            ApplyChange(new SampleDomainWithEventSourcingCreated(id, version));
        }

        public void ChangeVersion(string newVersion)
        {
            if (_published) throw new Exception("Can not change version because published");

            ApplyChange(new SampleDomainWithEventSourcingChangedVersion(Id, newVersion));
        }

        public void Publish()
        {
            ApplyChange(new SampleDomainWithEventSourcingPublished(Id));
        }

        public void Unpublish()
        {
            ApplyChange(new SampleDomainWithEventSourcingUnpublished(Id));
        }
    }
}
