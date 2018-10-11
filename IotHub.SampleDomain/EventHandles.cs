using IotHub.CommandsEvents.SampleDomain;
using IotHub.Core.Cqrs;
using System;

namespace IotHub.SampleDomain
{
    public class SampleDomainEventHandle : IEventHandle<SampleEventCreated>
    {
        public void Handle(SampleEventCreated e)
        {
            Console.WriteLine("Subsriber SampleDomainEventHandle");
        }
    }

    public class SampleDomainWorkfollowEventHandle : IEventHandle<SampleEventCreated>
    {
        public void Handle(SampleEventCreated e)
        {
            Console.WriteLine("Subsriber SampleDomainWorkfollowEventHandle");
        }
    }
}
