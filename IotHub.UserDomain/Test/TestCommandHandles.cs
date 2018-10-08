using IotHub.CommandsEvents.UserDomain.Test;
using IotHub.Core.Cqrs;
using IotHub.Core.Cqrs.EventSourcingRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace IotHub.UserDomain.Test
{
    public class TestCommandHandles : ICommandHandle<TestCommand>
    {
        public ICqrsEventSourcingRepository Repository { get; }

        public void Handle(TestCommand c)
        {
            Console.WriteLine(DateTime.Now);
        }
    }
}
