using IotHub.CommandsEvents.SampleDomain;
using IotHub.Core;
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
            var redishost = ConfigurationManagerExtensions.GetConnectionString("RedisConnectionString");
            RedisServices.Init(redishost, null, string.Empty);
          
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
                        MessiveSendCmd(null);
                        break;
                    case "pubsub":
                        EngineeEventWorkerQueue.Push(new SampleEventCreated()
                        {
                            PublishedEventId = Guid.NewGuid(),
                            SampleVersion = DateTime.Now.ToString(),
                            Version = 0
                        });
                        break;
                    case "pubsubmad":
                        _stop = false;
                        MessiveSendCmd(()=> {
                            EngineeEventWorkerQueue.Push(new SampleEventCreated()
                            {
                                PublishedEventId = Guid.NewGuid(),
                                SampleVersion = DateTime.Now.ToString(),
                                Version = 0
                            });
                        });
                        break;
                }
                cmd = (Console.ReadLine() ?? string.Empty).ToLower().Trim();
            }

            Console.Read();
        }

        private static void MessiveSendCmd(Action a)
        {
            new Thread(() =>
            {
                while (true)
                {
                    if (_stop) return;
                    if (a == null)
                    {
                        CommandEventSender.Send(new PingWorker(DateTime.Now.ToString())
                        {
                            PublishedCommandId = Guid.NewGuid(),
                            TokenSession = Guid.NewGuid().ToString()
                        });
                    }
                    else
                    {
                        a();
                    }
                }
            }).Start();
        }
    }
}
