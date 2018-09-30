using IotHub.Core.Cqrs;

namespace IotHub.Core.CqrsEngine
{
    public static class CommandEventSender
    {
        public static void Send(ICommand cmd, string tokenSession = "", bool asyncExec = true)
        {
            if (tokenSession != "")
            {
                cmd.TokenSession = tokenSession;
            }
            CommandsAndEventsRegisterEngine.PushCommand(cmd, asyncExec);
        }

        public static void Send(IEvent evt, bool asyncExec = true)
        {
            CommandsAndEventsRegisterEngine.PushEvent(evt, asyncExec);
        }
    }
}
