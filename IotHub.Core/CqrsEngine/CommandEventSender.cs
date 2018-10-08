using IotHub.Core.Cqrs;
using System;

namespace IotHub.Core.CqrsEngine
{
    public static class CommandEventSender
    {
        public static void Send(ICommand cmd, bool asyncExec = true)
        {
            if (cmd.PublishedCommandId == Guid.Empty)
            {
                cmd.PublishedCommandId = Guid.NewGuid();
            }
            CommandsAndEventsRegisterEngine.PushCommand(cmd, asyncExec);
        }

        public static void Send(IEvent evt, bool asyncExec = true)
        {
            if (evt.PublishedEventId == Guid.Empty)
            {
                evt.PublishedEventId = Guid.NewGuid();
            }
            CommandsAndEventsRegisterEngine.PushEvent(evt, asyncExec);
        }
    }
}
