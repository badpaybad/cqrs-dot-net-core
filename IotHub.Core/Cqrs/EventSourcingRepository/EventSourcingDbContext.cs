using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IotHub.Core.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace IotHub.Core.Cqrs.EventSourcingRepository
{
    internal class EventSourcingDbContext :  BaseMsSqlDbContext
    {
        public EventSourcingDbContext(string connectionString) : base(connectionString)
        {
        }

        public DbSet<EventSourcingDescription> EventSoucings { get; set; }
    }

    [Table("EventSourcingDescription")]
    public class EventSourcingDescription
    {
        [Key]
        public Guid EsdId { get; set; }

        public string AggregateId { get; set; }

        public long Version { get; set; }

        [StringLength(512)]
        public string AggregateType { get; set; } = string.Empty;

        [StringLength(512)]
        public string EventType { get; set; } = string.Empty;

        public string EventData { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}