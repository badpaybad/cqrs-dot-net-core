using IotHub.Core.Cqrs;
using IotHub.Core.Redis;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace IotHub.Core.CqrsEngine
{
    public static class EngineeCommandWorkerQueue
    {
        //in-memory queue, can be use redis queue, rabitmq ...
        // remember dispatched by type of command
        static readonly ConcurrentDictionary<string, ConcurrentQueue<ICommand>> _cmdDataQueue = new ConcurrentDictionary<string, ConcurrentQueue<ICommand>>();

        static readonly ConcurrentDictionary<string, List<Thread>> _cmdWorker = new ConcurrentDictionary<string, List<Thread>>();
        static readonly ConcurrentDictionary<string, bool> _stopWorker = new ConcurrentDictionary<string, bool>();
        static readonly ConcurrentDictionary<string, int> _workerCounterStoped = new ConcurrentDictionary<string, int>();
        static readonly ConcurrentDictionary<string, bool> _workerStoped = new ConcurrentDictionary<string, bool>();
        static readonly ConcurrentDictionary<string, Type> _cmdTypeName = new ConcurrentDictionary<string, Type>();
        static readonly object _locker = new object();

        public static void Push(ICommand cmd)
        {
            var type = cmd.GetType().FullName;

            if (RedisServices.IsEnable)
            {
                var queueName = BuildRedisQueueName(type);

                if (RedisServices.RedisDatabase.KeyExists(queueName))
                {
                    RedisServices.RedisDatabase
                        .ListLeftPush(queueName, JsonConvert.SerializeObject(cmd));
                }
                else
                {
                    // _cmdTypeName[type] = type;

                    RedisServices.RedisDatabase
                        .ListLeftPush(queueName, JsonConvert.SerializeObject(cmd));

                    InitFirstWorker(type);
                }
            }
            else
            {
                if (_cmdDataQueue.ContainsKey(type) && _cmdDataQueue[type] != null)
                {
                    //in-memory queue, can be use redis queue, rabitmq ...
                    _cmdDataQueue[type].Enqueue(cmd);
                }
                else
                {
                    //_cmdTypeName[type] = type;

                    //in-memory queue, can be use redis queue, rabitmq ...
                    _cmdDataQueue[type] = new ConcurrentQueue<ICommand>();
                    _cmdDataQueue[type].Enqueue(cmd);

                    InitFirstWorker(type);
                }
            }

        }

        private static string BuildRedisQueueName(string type)
        {
            return "EngineeCommandWorkerQueue_" + type;
        }

        private static void InitFirstWorker(string type)
        {
            while (_stopWorker.ContainsKey(type) && _stopWorker[type])
            {
                Thread.Sleep(100);
                //wait stopping
            }

            lock (_locker)
            {

                if (!_cmdWorker.ContainsKey(type) || _cmdWorker[type] == null || _cmdWorker[type].Count == 0)
                {
                    _stopWorker[type] = false;
                    _workerCounterStoped[type] = 0;
                    _workerStoped[type] = false;

                    _cmdWorker[type] = new List<Thread>();
                }

                var firstThread = new Thread(() => { WorkerDo(type); });

                _cmdWorker[type].Add(firstThread);

                firstThread.Start();
            }
        }

        static EngineeCommandWorkerQueue()
        {

        }

        static void WorkerDo(string type)
        {
            while (true)
            {
                try
                {
                    while (_stopWorker.ContainsKey(type) == false || _stopWorker[type] == false)
                    {
                        try
                        {
                            var canDequeue = CommandsAndEventsRegisterEngine.CommandWorkerCanDequeue(type);
                            if (!canDequeue)
                            {
                                continue;
                            }
                            if (RedisServices.IsEnable)
                            {
                                var queueName = BuildRedisQueueName(type);

                                var typeRegistered = CommandsAndEventsRegisterEngine.FindTypeOfCommandOrEvent(type);

                                var cmdJson = RedisServices.RedisDatabase
                                    .ListRightPop(queueName);
                                if (cmdJson.HasValue)
                                {
                                    var cmd = JsonConvert.DeserializeObject(cmdJson, typeRegistered) as ICommand;
                                    if (cmd != null)
                                    {
                                        try
                                        {
                                            CommandsAndEventsRegisterEngine.ExecCommand(cmd);
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine(ex.Message);
                                            RedisServices.RedisDatabase
                                       .ListLeftPush(queueName, cmdJson);
                                        }
                                    }
                                    else
                                    {
                                        RedisServices.RedisDatabase
                                        .ListLeftPush(queueName, cmdJson);
                                    }
                                }
                            }
                            else
                            {
                                if (_cmdDataQueue.TryGetValue(type, out ConcurrentQueue<ICommand> cmdQueue) &&
                                    cmdQueue != null)
                                {
                                    //in-memory queue, can be use redis queue, rabitmq ...
                                    if (cmdQueue.TryDequeue(out ICommand cmd) && cmd != null)
                                    {
                                        CommandsAndEventsRegisterEngine.ExecCommand(cmd);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        finally
                        {
                            Thread.Sleep(0);
                        }
                    }

                    if (!_workerCounterStoped.ContainsKey(type))
                    {
                        _workerCounterStoped[type] = 0;
                    }
                    if (_workerStoped[type] == false)
                    {
                        var counter = _workerCounterStoped[type];
                        counter++;
                        _workerCounterStoped[type] = counter;

                        lock (_locker)
                        {
                            if (_cmdWorker.TryGetValue(type, out List<Thread> listThread))
                            {
                                if (listThread.Count == counter)
                                {
                                    _workerStoped[type] = true;
                                    _workerCounterStoped[type] = 0;
                                }
                            }
                        }
                    }
                }
                finally
                {
                    Thread.Sleep(100);
                }
            }
        }

        /// <summary>
        /// reset thread to one. each command have one thread to process
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool ResetToOneWorker(string type)
        {
            _stopWorker[type] = true;

            while (!_workerStoped.ContainsKey(type) || _workerStoped[type] == false)
            {
                Thread.Sleep(100);
                //wait all worker done its job
            }

            //List<Thread> threads;

            //if (_cmdWorker.TryGetValue(type, out threads))
            //{
            //    foreach (var t in threads)
            //    {
            //        try
            //        {
            //            t.Abort();
            //        }
            //        catch { }
            //    }
            //}

            _workerCounterStoped[type] = 0;
            _workerStoped[type] = false;
            _cmdWorker[type].Clear();
            _stopWorker[type] = false;

            InitFirstWorker(type);

            return true;
        }

        public static bool AddAndStartWorker(string type)
        {
            if (!_cmdWorker.ContainsKey(type) || _cmdWorker[type] == null || _cmdWorker[type].Count == 0)
            {
                InitFirstWorker(type);
            }
            else
            {
                lock (_locker)
                {
                    _workerStoped[type] = false;
                    var thread = new Thread(() => WorkerDo(type));
                    _cmdWorker[type].Add(thread);
                    thread.Start();
                }
            }

            return true;
        }

        public static void CountStatistic(string type, out int queueDataCount, out int workerCount)
        {
            queueDataCount = 0;
            workerCount = 0;
            if (_cmdWorker.TryGetValue(type, out List<Thread> list) && list != null)
            {
                workerCount = list.Count;
            }
            if (RedisServices.IsEnable)
            {
                var queueName = BuildRedisQueueName(type);

                queueDataCount = (int)RedisServices.RedisDatabase.ListLength(queueName);
            }
            else
            {
                if (_cmdDataQueue.TryGetValue(type, out ConcurrentQueue<ICommand> queue) && queue != null)
                {
                    queueDataCount = queue.Count;
                }
            }
        }

        public static bool IsWorkerStopping(string type)
        {
            bool val;
            if (_stopWorker.TryGetValue(type, out val))
            {
                return val;
            }

            return false;
        }

        public static void Start()
        {
            var listCmd = CommandsAndEventsRegisterEngine._commandsEvents.Values
               .Where(i => typeof(ICommand).IsAssignableFrom(i)).ToList();

            foreach (var t in listCmd)
            {
                InitFirstWorker(t.FullName);
            }
        }

        public static List<string> ListAllCommandName()
        {
            lock (_locker)
            {
                return _cmdTypeName.Select(i => i.Key).ToList();
            }
        }

        public static Type GetType(string fullName)
        {
            lock (_locker)
            {
                return _cmdTypeName[fullName];
            }
        }
    }

}
