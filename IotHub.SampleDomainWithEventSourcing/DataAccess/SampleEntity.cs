using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace IotHub.SampleDomainWithEventSourcing.DataAccess
{

    public class SampleEntity
    {
        [Key]
        public Guid Id { get; set; }

        public string Version { get; set; }

        public bool Published { get; set; }
    }
}
