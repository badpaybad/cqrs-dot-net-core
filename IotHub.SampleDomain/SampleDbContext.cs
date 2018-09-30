using IotHub.Core;
using IotHub.Core.DataAccess;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace IotHub.SampleDomain
{
    public class SampleDbContext : AbstractMsSqlDbContext
    {       
        public SampleDbContext() 
            : base(ConfigurationManagerExtensions.GetConnectionString("SampleDbContext"))
        {
        }

        public DbSet<SampleEntity> SampleEntities { get; set; }
    }

    public class SampleEntity
    {
        [Key]
        public Guid Id { get; set; }

        public string Version { get; set; }
    }
}
