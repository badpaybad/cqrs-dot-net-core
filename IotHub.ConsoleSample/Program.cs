using IotHub.Core.CqrsEngine;
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
            CommandsAndEventsRegisterEngine.Init("Server=.\\SQLEXPRESS;Database=iothub;Trusted_Connection=True;");
            CommandsAndEventsRegisterEngine.AutoRegister();
            MessiveSendCmd();

            FakeProcess();

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

        private static void FakeProcess()
        {
            EngineeCommandWorkerQueue.Start();
            EngineeEventWorkerQueue.Start();
        }

        private static void MessiveSendCmd()
        {
            new Thread(()=> {
                while (true)
                {
                    if (_stop) return;

                    CommandEventSender.Send(new PingWorker(DateTime.Now.ToString()) {
                        CommandId= Guid.NewGuid(),
                        TokenSession= Guid.NewGuid().ToString()
                    });
                }
            }).Start();            
        }
    }
}
