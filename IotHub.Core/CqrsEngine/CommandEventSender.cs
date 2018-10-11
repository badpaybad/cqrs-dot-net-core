using IotHub.Core.Cqrs;
using IotHub.Core.Redis;
using System;

namespace IotHub.Core.CqrsEngine
{
    public static class CommandEventSender
    {
        public static void Send(ICommand cmd)
        {
            if (cmd.PublishedCommandId == Guid.Empty)
            {
                cmd.PublishedCommandId = Guid.NewGuid();
            }
            CommandsAndEventsRegisterEngine.PushCommand(cmd, RedisServices.IsEnable);
        }

        public static void Send(IEvent evt)
        {
            if (evt.PublishedEventId == Guid.Empty)
            {
                evt.PublishedEventId = Guid.NewGuid();
            }
            CommandsAndEventsRegisterEngine.PushEvent(evt, RedisServices.IsEnable);
        }
    }
}
