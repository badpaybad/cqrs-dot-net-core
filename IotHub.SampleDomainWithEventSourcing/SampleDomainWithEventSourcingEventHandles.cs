using IotHub.CommandsEvents.SampleDomainWithEventSourcing;
using IotHub.Core;
using IotHub.Core.Cqrs;
using IotHub.Core.Cqrs.EventSourcingRepository;
using IotHub.Core.DataAccess;
using IotHub.SampleDomainWithEventSourcing.DataAccess;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace IotHub.SampleDomainWithEventSourcing
{
    public class SampleDomainWithEventSourcingEventHandles :
         IEventHandle<SampleDomainWithEventSourcingCreated>,
         IEventHandle<SampleDomainWithEventSourcingChangedVersion>,
         IEventHandle<SampleDomainWithEventSourcingPublished>,
         IEventHandle<SampleDomainWithEventSourcingUnpublished>
    {
        public ICqrsEventSourcingRepository Repository { get; }

        public void Handle(SampleDomainWithEventSourcingUnpublished e)
        {
            using (var db = new SampleDbContext())
            {
                var s = new SampleEntity()
                {
                    Id = e.Id,
                    Published = false
                };
                var entry = db.Entry<SampleEntity>(s);
                db.Attach(entry);
                entry.Property(p => p.Published).IsModified = true;
                db.SaveChanges();
            }
        }

        public void Handle(SampleDomainWithEventSourcingPublished e)
        {
            using (var db = new SampleDbContext())
            {
                var s = new SampleEntity()
                {
                    Id = e.Id,
                    Published = true
                };
                var entry = db.Entry<SampleEntity>(s);
                db.Attach(entry);
                entry.Property(p => p.Published).IsModified = true;
                db.SaveChanges();
            }
        }

        public void Handle(SampleDomainWithEventSourcingChangedVersion e)
        {
            using (var db = new SampleDbContext())
            {
                var s = new SampleEntity()
                {
                    Id = e.Id,
                    Version = e.SampleVersion
                };
                var entry = db.Entry<SampleEntity>(s);
                db.Attach(entry);
                entry.Property(p => p.Version).IsModified = true;
                db.SaveChanges();
            }
        }

        public void Handle(SampleDomainWithEventSourcingCreated e)
        {
            using (var db = new SampleDbContext())
            {
                db.SampleEntities.Add(new SampleEntity()
                {
                    Id = e.Id,
                    Version = e.SampleVersion
                });
                db.SaveChanges();
            }
        }


    }
}
