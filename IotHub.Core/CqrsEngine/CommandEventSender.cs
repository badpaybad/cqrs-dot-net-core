﻿using IotHub.Core.Cqrs;
using IotHub.Core.Redis;
using Newtonsoft.Json;
using System;

namespace IotHub.Core.CqrsEngine
{
    public static class CommandEventSender
    {
        public static void Send(ICommand cmd)
        {
            if (cmd.PublishedCommandId == null || cmd.PublishedCommandId == Guid.Empty)
            {
                cmd.PublishedCommandId = Guid.NewGuid();
            }
            CommandsAndEventsRegisterEngine.PushCommand(cmd, RedisServices.IsEnable);
            Console.WriteLine("Sent");
            Console.WriteLine(JsonConvert.SerializeObject(cmd));
        }

        public static void Send(IEvent evt)
        {
            if (evt.PublishedEventId == null || evt.PublishedEventId == Guid.Empty)
            {
                evt.PublishedEventId = Guid.NewGuid();
            }
            CommandsAndEventsRegisterEngine.PushEvent(evt, RedisServices.IsEnable);
            Console.WriteLine("Sent");
            Console.WriteLine(JsonConvert.SerializeObject(evt));
        }
    }
}
