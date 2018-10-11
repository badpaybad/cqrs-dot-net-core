using IotHub.CommandsEvents.SampleDomain;
using IotHub.Core.CqrsEngine;
using IotHub.Core.PingDomain;
using IotHub.Core.Redis;
using System;
using System.Threading;

namespace IotHub.ConsoleSample
{
    class Program
    {
        static bool _stop;
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            try
            {
                Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.WriteLine("press Enter to quit");
            Console.ReadLine();
            Environment.Exit(0);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject;
            if (ex != null)
            {
                Console.WriteLine(ex);
                Console.ReadLine();
            }
        }

        private static void Run()
        {
            RedisServices.Init("127.0.0.1", null, string.Empty);

            CommandsAndEventsRegisterEngine.AutoRegister();

            EngineeCommandWorkerQueue.Start();
            EngineeEventWorkerQueue.Start();
            var cmd = (Console.ReadLine() ?? string.Empty).ToLower().Trim();

            while (!cmd.Equals("quit"))
            {
                switch (cmd)
                {
                    case "quit":
                        _stop = true;
                        break;
                    case "stop":
                        _stop = true;
                        break;
                    case "start":
                        _stop = false;
                        break;
                    case "pubsub":
                        EngineeEventWorkerQueue.Push(new SampleEventCreated()
                        {
                            PublishedEventId = Guid.NewGuid(),
                            SampleVersion = DateTime.Now.ToString(),
                            Version = 0
                        });
                        break;
                }
                cmd = (Console.ReadLine() ?? string.Empty).ToLower().Trim();
            }

            Console.Read();
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
                        PublishedCommandId = Guid.NewGuid(),
                        TokenSession = Guid.NewGuid().ToString()
                    });
                }
            }).Start();
        }
    }
}
