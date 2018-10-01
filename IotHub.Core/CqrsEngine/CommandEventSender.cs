using IotHub.Core.Cqrs;
using System;

namespace IotHub.Core.CqrsEngine
{
    public static class CommandEventSender
    {
        public static void Send(ICommand cmd, bool asyncExec = true)
        {
            if (cmd.CommandId == Guid.Empty)
            {
                cmd.CommandId = Guid.NewGuid();
            }
            CommandsAndEventsRegisterEngine.PushCommand(cmd, asyncExec);
        }

        public static void Send(IEvent evt, bool asyncExec = true)
        {
            if (evt.EventId == Guid.Empty)
            {
                evt.EventId = Guid.NewGuid();
            }
            CommandsAndEventsRegisterEngine.PushEvent(evt, asyncExec);
        }
    }
}
