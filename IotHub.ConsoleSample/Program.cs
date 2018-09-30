using IotHub.Core.CqrsEngine;
using IotHub.Core.Redis;
using IotHub.Core.SampleHandles;
using System;
using System.Threading;

namespace IotHub.ConsoleSample
{
    class Program
    {
        static bool _stop;
        static void Main(string[] args)
        {
            RedisServices.Init("127.0.0.1", null, string.Empty);

            CommandsAndEventsRegisterEngine.AutoRegister();

            EngineeCommandWorkerQueue.Start();
            EngineeEventWorkerQueue.Start();

            while (true)
            {
                var cmd = Console.ReadLine();
                switch (cmd.ToLower())
                {
                    case "quit":
                        _stop = true;
                        Environment.Exit(0);
                        break;
                    case "stop":
                        _stop = true;
                        break;
                    case "start":
                        _stop = false;
                        MessiveSendCmd();
                        break;
                }
            }
        }

      
        private static void MessiveSendCmd()
        {
            new Thread(() =>
            {
                while (true)
                {
                    if (_stop) return;

                    CommandEventSender.Send(new PingWorker(DateTime.Now.ToString())
                    {
                        CommandId = Guid.NewGuid(),
                        TokenSession = Guid.NewGuid().ToString()
                    });
                }
            }).Start();
        }
    }
}
