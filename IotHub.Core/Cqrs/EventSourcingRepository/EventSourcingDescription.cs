using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IotHub.Core.Cqrs.EventSourcingRepository
{
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

    [Table("CommandLog")]
    public class CommandLog
    {
        [Key]
        public string CsdId { get; set; }

        [StringLength(512)]
        public string CommandType { get; set; }

        public string CommandData { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}