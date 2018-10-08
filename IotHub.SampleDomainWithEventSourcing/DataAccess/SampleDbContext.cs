using IotHub.Core;
using IotHub.Core.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace IotHub.SampleDomainWithEventSourcing.DataAccess
{
    public class SampleDbContext : AbstractMsSqlDbContext
    {
        public SampleDbContext()
            : base(ConfigurationManagerExtensions.GetConnectionString("SampleDbContext"))
        {
        }

        public DbSet<SampleEntity> SampleEntities { get; set; }
    }
}
