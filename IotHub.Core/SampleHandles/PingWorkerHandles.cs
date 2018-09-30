using IotHub.Core.Cqrs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace IotHub.Core.SampleHandles
{
    public class PingWorker : ICommand
    {
        public readonly string Data;

        public PingWorker(string data)
        {
            Data = data;
        }

        public Guid CommandId { get; set; } = Guid.NewGuid();
        public string TokenSession { get; set; }
    }

    public class PingWorkerCommandHandles : ICommandHandle<PingWorker>
    {
        private static object _locker = new object();
        Random _rnd = new Random();
        public void Handle(PingWorker c)
        {
            try
            {
                var appData = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data");
                if (Directory.Exists(appData) == false)
                {
                    Directory.CreateDirectory(appData);
                }

                var log = Path.Combine(appData, "pingworker.log");
                lock (_locker)
                {
                    using (var sw = new StreamWriter(log, true))
                    {
                        var dateNow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        sw.WriteLine($"{dateNow} " + c.Data);
                        sw.Flush();
                    }
                }

                Thread.Sleep(_rnd.Next(3000));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }

}
