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
    public static class EngineeEventWorkerQueue
    {
        //in-memory queue, can be use redis queue, rabitmq ...
        // remember dispatched by type of event
        static readonly ConcurrentDictionary<string, ConcurrentQueue<IEvent>> _evtDataQueue = new ConcurrentDictionary<string, ConcurrentQueue<IEvent>>();

        static readonly ConcurrentDictionary<string, List<Thread>> _evtWorker = new ConcurrentDictionary<string, List<Thread>>();
        static readonly ConcurrentDictionary<string, bool> _stopWorker = new ConcurrentDictionary<string, bool>();
        static readonly ConcurrentDictionary<string, int> _workerCounterStoped = new ConcurrentDictionary<string, int>();
        static readonly ConcurrentDictionary<string, bool> _workerStoped = new ConcurrentDictionary<string, bool>();
        static readonly ConcurrentDictionary<string, Type> _evtTypeName = new ConcurrentDictionary<string, Type>();
        static readonly object _locker = new object();

        static readonly string ListEventTypeNameRedisKey = "EngineeEventWorkerQueue_ListEventTypeName";
        
        internal static void Push(IEvent evt)
        {
            var type = evt.GetType().FullName;
            _evtTypeName[type] = evt.GetType();

            if (RedisServices.IsEnable)
            {
                var queueName = BuildRedisQueueName(type);

                RedisServices.RedisDatabase.HashSet(ListEventTypeNameRedisKey, queueName, queueName);

                if (RedisServices.RedisDatabase.KeyExists(queueName))
                {
                    RedisServices.RedisDatabase
                        .ListLeftPush(queueName, JsonConvert.SerializeObject(evt));
                }
                else
                {
                    RedisServices.RedisDatabase
                        .ListLeftPush(queueName, JsonConvert.SerializeObject(evt));

                    InitFirstWorker(type);
                }
            }
            else
            {
                if (_evtDataQueue.ContainsKey(type) && _evtDataQueue[type] != null)
                {
                    _evtDataQueue[type].Enqueue(evt);
                }
                else
                {
                    _evtDataQueue[type] = new ConcurrentQueue<IEvent>();
                    _evtDataQueue[type].Enqueue(evt);

                    InitFirstWorker(type);
                }
            }

        }

        private static string BuildRedisQueueName(string type)
        {
            return "EngineeEventWorkerQueue_" + type;
        }

        static string BuildRedisChannelName(string type)
        {
            return "EngineeEventChannel_" + type;
        }

        static string BuildRedisTopicName(string channel, string subscribe)
        {
            return channel + "_" + subscribe;
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

                if (!_evtWorker.ContainsKey(type) || _evtWorker[type] == null || _evtWorker[type].Count == 0)
                {
                    _stopWorker[type] = false;
                    _workerCounterStoped[type] = 0;
                    _workerStoped[type] = false;

                    _evtWorker[type] = new List<Thread>();
                }

                var firstThread = new Thread(() => { WorkerDo(type); });

                _evtWorker[type].Add(firstThread);

                firstThread.Start();
            }
        }

        static EngineeEventWorkerQueue()
        {

        }

        public static void UnsubscribeAll(string typeEvent)
        {
            if (RedisServices.IsEnable == false)
            {
                Console.WriteLine("Should use redis for pubsub function");
                return;
            }

            var channel = BuildRedisChannelName(typeEvent);
            var allSubscribe = RedisServices.RedisDatabase.HashGetAll(channel);

            foreach (var s in allSubscribe)
            {
                var topic = BuildRedisTopicName(channel, s.Name);
                RedisServices.RedisSubscriber.Unsubscribe(topic);

                RedisServices.RedisDatabase.HashDelete(channel, s.Name);
            }

            RedisServices.RedisDatabase.KeyDelete(channel);

            Console.WriteLine($"UnSubscribe all for channel: {channel}");
        }

        public static void Unsubscribe(string typeEvent, string subscriberName)
        {
            if (RedisServices.IsEnable == false)
            {
                Console.WriteLine("Should use redis for pubsub function");
                return;
            }
            
            var channel = BuildRedisChannelName(typeEvent);
            var topic = BuildRedisTopicName(channel, subscriberName);
            RedisServices.RedisSubscriber.Unsubscribe(topic);
            RedisServices.RedisDatabase.HashDelete(channel, subscriberName);

            Console.WriteLine($"Unsubscribe for topic: {topic}");
        }

        public static void Subscribe(string typeEvent, Action<object> handle, string subscriberName)
        {
            if (RedisServices.IsEnable == false)
            {
                Console.WriteLine("Should use redis for pubsub function");
                return;
            }

            var channel = BuildRedisChannelName(typeEvent);

            if (RedisServices.RedisDatabase.HashGet(channel, subscriberName).HasValue)
            {
                Unsubscribe(typeEvent, subscriberName);
                //return;
            }

            RedisServices.RedisDatabase.HashSet(channel, subscriberName, subscriberName);

            var topic = BuildRedisTopicName(channel, subscriberName);

            Redis.RedisServices.RedisSubscriber.Subscribe(topic, (c, evtJson) =>
            {
                try
                {
                    var typeRegistered = CommandsAndEventsRegisterEngine.FindTypeOfCommandOrEvent(typeEvent);

                    if (evtJson.HasValue)
                    {
                        var evt = JsonConvert.DeserializeObject(evtJson, typeRegistered) as IEvent;
                        if (evt != null)
                        {
                            handle(evt);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error subscriber {subscriberName} at channel {c} {ex.Message}");
                }
            });

            Console.WriteLine($"Subscribe for topic: {topic}");
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
                            if (!CommandsAndEventsRegisterEngine.EventWorkerCanDequeue(type))
                            {
                                Thread.Sleep(100);
                                continue;
                            }
                            if (RedisServices.IsEnable)
                            {
                                var channel = BuildRedisChannelName(type);

                                var allSubscribe = RedisServices.RedisDatabase.HashGetAll(channel);

                                if (allSubscribe.Count() <= 0)
                                {
                                    Console.WriteLine("No consummer to process event data");
                                    Thread.Sleep(100);
                                    continue;
                                }

                                var queueName = BuildRedisQueueName(type);
                                var typeRegistered = CommandsAndEventsRegisterEngine.FindTypeOfCommandOrEvent(type);
                                var evtJson = RedisServices.RedisDatabase.ListRightPop(queueName);
                                if (evtJson.HasValue)
                                {
                                    try
                                    {
                                        var evt = JsonConvert.DeserializeObject(evtJson, typeRegistered) as IEvent;
                                        if (evt != null)
                                        {
                                            foreach (var subscriber in allSubscribe)
                                            {
                                                var topic = BuildRedisTopicName(channel, subscriber.Name);

                                                RedisServices.RedisSubscriber.Publish(topic, evtJson);
                                            }
                                        }
                                        else
                                        {
                                            RedisServices.RedisDatabase.ListLeftPush(queueName, evtJson);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        RedisServices.RedisDatabase.ListLeftPush(queueName, evtJson);

                                        Console.WriteLine(ex.Message);
                                    }

                                }
                            }
                            else
                            {
                                if (_evtDataQueue.TryGetValue(type, out ConcurrentQueue<IEvent> evtQueue) &&
                                    evtQueue != null)
                                {
                                    //in-memory queue, can be use redis queue, rabitmq ...
                                    if (evtQueue.TryDequeue(out IEvent evt) && evt != null)
                                    {
                                        CommandsAndEventsRegisterEngine.ExecEvent(evt);
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
                            Thread.Sleep(100);
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
                            if (_evtWorker.TryGetValue(type, out List<Thread> listThread))
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
            _evtWorker[type].Clear();
            _stopWorker[type] = false;

            InitFirstWorker(type);

            return true;
        }

        public static bool AddAndStartWorker(string type)
        {
            if (!_evtWorker.ContainsKey(type) || _evtWorker[type] == null || _evtWorker[type].Count == 0)
            {
                InitFirstWorker(type);
            }
            else
            {
                lock (_locker)
                {
                    _workerStoped[type] = false;
                    var thread = new Thread(() => WorkerDo(type));
                    _evtWorker[type].Add(thread);
                    thread.Start();
                }
            }

            return true;
        }

        public static void CountStatistic(string type, out int queueDataCount, out int workerCount)
        {
            queueDataCount = 0;
            workerCount = 0;
            if (_evtWorker.TryGetValue(type, out List<Thread> list) && list != null)
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
                if (_evtDataQueue.TryGetValue(type, out ConcurrentQueue<IEvent> queue) && queue != null)
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
            var listEvt = CommandsAndEventsRegisterEngine._commandsEvents.Values
                          .Where(i => typeof(IEvent).IsAssignableFrom(i)).ToList();

            //var listSub = CommandsAndEventsRegisterEngine.GetAllSubscriber();

            //foreach (var t in listEvt)
            //{
            //    var channel = BuildRedisChannelName(t.FullName);

            //    foreach (var s in listSub)
            //    {
            //        RedisServices.RedisDatabase.HashDelete(channel, s);
            //    }
            //}

            foreach (var t in listEvt)
            {
                InitFirstWorker(t.FullName);

            }
        }

        public static List<string> ListAllCommandName()
        {
            if (RedisServices.IsEnable)
            {
                return RedisServices.RedisDatabase.HashGetAll(ListEventTypeNameRedisKey)
                  .Select(i => i.Name.ToString()).ToList();
            }

            lock (_locker)
            {
                return _evtTypeName.Select(i => i.Key).ToList();
            }
        }

        public static Type GetType(string fullName)
        {
            lock (_locker)
            {
                return _evtTypeName[fullName];
            }
        }
    }
}
