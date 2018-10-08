using IotHub.CommandsEvents.SampleDomainWithEventSourcing;
using IotHub.Core.Cqrs;
using IotHub.Core.Cqrs.EventSourcingRepository;

namespace IotHub.SampleDomainWithEventSourcing
{
    public class SampleDomainWithEventSourcingCommandHandles :
        ICommandHandle<CreateSampleDomainWithEventSourcing>,
        ICommandHandle<ChangeVersionSampleDomainWithEventSourcing>,
        ICommandHandle<PublishSampleDomainWithEventSourcing>,
        ICommandHandle<UnpublishSampleDomainWithEventSourcing>
    {
        public ICqrsEventSourcingRepository Repository
            = new CqrsEventSourcingRepository(new EventPublisher());

        ICqrsEventSourcingRepository ICqrsHandle.Repository { get; }
            = new CqrsEventSourcingRepository(new EventPublisher());

        public void Handle(CreateSampleDomainWithEventSourcing c)
        {
            var s = new SampleDomainAggregateRoot();
            s.Create(c.Id, c.Version);
            Repository.CreateNew(s);
        }

        public void Handle(ChangeVersionSampleDomainWithEventSourcing c)
        {
            Repository.GetDoSave<SampleDomainAggregateRoot>(c.Id,
                (a) => a.ChangeVersion(c.Version));
        }

        public void Handle(PublishSampleDomainWithEventSourcing c)
        {
            var s = Repository.Get<SampleDomainAggregateRoot>(c.Id);
            s.Publish();
            Repository.Save(s);
        }

        public void Handle(UnpublishSampleDomainWithEventSourcing c)
        {
            Repository.GetDoSave<SampleDomainAggregateRoot>(c.Id,
               (a) => a.Publish());
        }
    }
}
