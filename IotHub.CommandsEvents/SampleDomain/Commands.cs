using IotHub.Core.Cqrs;
using System;
using System.Collections.Generic;
using System.Text;

namespace IotHub.CommandsEvents.SampleDomain
{
    public class CreateSample : ICommand
    {
        public Guid CommandId { get; set; }
        public string TokenSession { get; set; }

        public Guid SampleId { get; set; }
        public string Version { get; set; }
    }

}
