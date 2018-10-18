using IotHub.CommandsEvents.SampleDomain;
using IotHub.Core.Cqrs;
using IotHub.Core.Cqrs.EventSourcingRepository;
using IotHub.Core.CqrsEngine;
using Newtonsoft.Json;
using System;

namespace IotHub.SampleDomainWithEventSourcing.NoEventSourcing
{  
    public class SampleHandles : ICommandHandle<CreateSample>
    {
        public ICqrsEventSourcingRepository Repository { get; }
        = new CqrsEventSourcingRepository(new EventPublisher());

        public void Handle(CreateSample c)
        {
            var temp = JsonConvert.SerializeObject(c);
            //no event sourcing do manipulate db directly
            //using (var db = new SampleDbContext())
            //{
                
            //    db.SampleEntities.Add(new SampleEntity()
            //    {
            //        Id = Guid.NewGuid(),
            //        Version = temp
            //    });
            //    db.SaveChanges();
            //}
            Console.WriteLine("Call from api but Exec in console. Saved to db:" );
            Console.WriteLine(temp);

            CommandEventSender.Send(new SampleEventCreated()
            {
                PublishedEventId = Guid.NewGuid(),
                SampleVersion = $"Come from CreateSample command with cmdid {c.PublishedCommandId}"
            });
        }
    }
}
