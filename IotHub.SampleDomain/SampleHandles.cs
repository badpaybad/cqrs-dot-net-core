using IotHub.CommandsEvents.SampleDomain;
using IotHub.Core.Cqrs;
using IotHub.Core.Cqrs.EventSourcingRepository;
using Newtonsoft.Json;
using System;

namespace IotHub.SampleDomain
{  
    public class SampleHandles : ICommandHandle<CreateSample>
    {
        public ICqrsEventSourcingRepository Repository { get; }

        public void Handle(CreateSample c)
        {
            var temp = JsonConvert.SerializeObject(c);

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
        }
    }
}
