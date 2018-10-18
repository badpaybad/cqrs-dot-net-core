using IotHub.CommandsEvents.SampleDomain;
using IotHub.Core.Cqrs;
using Newtonsoft.Json;
using System;

namespace IotHub.SampleDomainWithEventSourcing.NoEventSourcing
{
    public class SampleDomainEventHandle : IEventHandle<SampleEventCreated>
    {
        public void Handle(SampleEventCreated e)
        {
            Console.WriteLine("Subsriber SampleDomainEventHandle");
            Console.WriteLine(JsonConvert.SerializeObject(e));
        }
    }

    public class SampleDomainWorkfollowEventHandle : IEventHandle<SampleEventCreated>
    {
        public void Handle(SampleEventCreated e)
        {
            Console.WriteLine("Subsriber SampleDomainWorkfollowEventHandle");
            Console.WriteLine(JsonConvert.SerializeObject(e));
        }
    }
}
