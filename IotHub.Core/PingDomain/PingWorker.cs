using IotHub.Core.Cqrs;
using System;
using System.Collections.Generic;
using System.Text;

namespace IotHub.Core.PingDomain
{
    public class PingWorker : ICommand
    {
        public readonly string Data;

        public PingWorker(string data)
        {
            Data = data;
        }

        public Guid? PublishedCommandId { get; set; } = Guid.NewGuid();
        public string TokenSession { get; set; }
    }

}
