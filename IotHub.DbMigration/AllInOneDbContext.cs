using IotHub.Core.Cqrs.CqrsEngine;
using IotHub.Core.Cqrs.EventSourcingRepository;
using IotHub.Core.DataAccess;
using IotHub.SampleDomainWithEventSourcing.DataAccess;
using IotHub.UserDomain;
using Microsoft.EntityFrameworkCore;

namespace IotHub.DbMigration
{
    public class AllInOneDbContext : AbstractMsSqlDbContext
    {
        public AllInOneDbContext() : base("AllInOneDbContext")
        {

        }

        public DbSet<CommandEventStorage> CommandEventStorages { get; set; }
        public DbSet<CommandEventStorageHistory> CommandEventStorageHistories { get; set; }
        public DbSet<EventSourcingDescription> EventSoucings { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }

        public DbSet<SampleEntity> SampleEntities { get; set; }


        
    }
}
