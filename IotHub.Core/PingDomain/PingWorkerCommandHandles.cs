using IotHub.Core.Cqrs;
using IotHub.Core.Cqrs.EventSourcingRepository;
using System;
using System.IO;
using System.Threading;

namespace IotHub.Core.PingDomain
{
    public class PingWorkerCommandHandles : ICommandHandle<PingWorker>
    {
        public ICqrsEventSourcingRepository Repository { get; }

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
