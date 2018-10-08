﻿using IotHub.Core.Cqrs;
using System;
using System.Collections.Generic;
using System.Text;

namespace IotHub.CommandsEvents.UserDomain.Test
{
    public class TestCommand : ICommand
    {
        public Guid PublishedCommandId { get; set; }
        public string TokenSession { get; set; }
    }
}