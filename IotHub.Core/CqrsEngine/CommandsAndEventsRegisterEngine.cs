using IotHub.Core.Cqrs;
using IotHub.Core.Cqrs.CqrsEngine;
using IotHub.Core.Reflection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace IotHub.Core.CqrsEngine
{
    public static class CommandsAndEventsRegisterEngine
    {
        static readonly Dictionary<string, List<Action<IEvent>>> _eventHandler = new Dictionary<string, List<Action<IEvent>>>();
        static readonly Dictionary<string, Action<ICommand>> _commandHandler = new Dictionary<string, Action<ICommand>>();

        static object _eventLocker = new object();
        static object _commandLocker = new object();

        internal static readonly Dictionary<string, Type> _commandsEvents = new Dictionary<string, Type>();

        static CommandsAndEventsRegisterEngine()
        {
            //try
            //{
            //    using (var db = new CommandEventStorageDbContext())
            //    {
            //        db.Database.EnsureCreated();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception(ex.Message + $"Can not connect to db {nameof(CommandEventStorageDbContext)}", ex);
            //}
        }

        public static bool AutoRegister()
        {
            lock (_commandLocker)
            {
                _commandHandler.Clear();
            }

            lock (_eventLocker)
            {
                _eventHandler.Clear();
            }

            List<Assembly> allAss = FindAllDll();

            foreach (var assembly in allAss)
            {
                try
                {
                    RegisterAssembly(assembly);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Can not register assembly: " + assembly.FullName);
                    Console.WriteLine("- " + ex.GetAllMessages());
                }
            }

            return true;
        }

        private static List<Assembly> FindAllDll()
        {
            // return AppDomain.CurrentDomain.GetAssemblies();
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            List<Assembly> allAssemblies = new List<Assembly>();

            allAssemblies.AddRange(AppDomain.CurrentDomain.GetAssemblies());

            var dllFiles = Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories);
            foreach (string dll in dllFiles)
            {
                if (File.Exists(dll))
                {
                    try
                    {
                        allAssemblies.Add(Assembly.LoadFile(dll));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.GetAllMessages() + "\r\n" + "Can not load dll: " + dll);
                    }
                }
            }
            // return allAssemblies;

            Dictionary<string, Assembly> filter = new Dictionary<string, Assembly>();

            foreach (var a in allAssemblies)
            {
                filter[a.FullName] = a;
            }

            return filter.Values.ToList();
        }

        public static void RegisterAssembly(Assembly executingAssembly)
        {
            var allTypes = executingAssembly.GetTypes();

            RegisterCommandsEventsForWorker(allTypes);

            var listHandler = allTypes.Where(t => typeof(ICqrsHandle).IsAssignableFrom(t)
                                                  && t.IsClass && !t.IsAbstract).ToList();

            var assemblyFullName = executingAssembly.FullName;
            if (listHandler.Count <= 0)
            {
                Console.WriteLine("Not found ICqrsHandle in " + assemblyFullName);
                return;
            }

            Console.WriteLine(assemblyFullName);
            Console.WriteLine($"Found {listHandler.Count} handle(s) to register to message buss");

            foreach (var handlerType in listHandler)
            {
                var cqrsHandler = (ICqrsHandle)Activator.CreateInstance(handlerType);
                if (cqrsHandler == null) continue;

                Console.WriteLine($"Found Handle type: {cqrsHandler.GetType()}");

                MethodInfo[] allMethod = cqrsHandler.GetType()
                    .GetMethods(BindingFlags.Public | BindingFlags.Instance);

                foreach (var mi in allMethod)
                {
                    var methodName = mi.Name;
                    if (!methodName.Equals("handle", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    var pParameterType = mi.GetParameters().SingleOrDefault().ParameterType;

                    if (typeof(IEvent).IsAssignableFrom(pParameterType))
                    {
                        var t = pParameterType.FullName;

                        lock (_eventLocker)
                        {
                            RegisterCommandsEventsForWorker(pParameterType);

                            List<Action<IEvent>> ax;

                            if (_eventHandler.TryGetValue(t, out ax))
                            {
                                ax.Add(p =>
                                {
                                    mi.Invoke(cqrsHandler, new object[] { p });
                                });
                            }
                            else
                            {
                                ax = new List<Action<IEvent>>() {
                                    p =>
                                    {
                                        mi.Invoke(cqrsHandler, new object[] { p });
                                    } };
                            }

                            _eventHandler[t] = ax;
                        }

                        var className = mi.DeclaringType.FullName;

                        EngineeEventWorkerQueue.Subscribe(t, (o) =>
                        {
                            mi.Invoke(cqrsHandler, new object[] { o });
                        }, className);

                        Console.WriteLine($"Regsitered method to process event type: {pParameterType}");
                    }

                    if (typeof(ICommand).IsAssignableFrom(pParameterType))
                    {
                        lock (_commandLocker)
                        {
                            var t = pParameterType.FullName;
                            RegisterCommandsEventsForWorker(pParameterType);

                            Action<ICommand> ax;

                            if (_commandHandler.TryGetValue(t, out ax))
                            {
                                return;
                                // throw new Exception($"Should only one handle to cover type: {t}. Check DomainEngine.Boot");
                            }

                            _commandHandler[t] = (p) =>
                            {
                                mi.Invoke(cqrsHandler, new object[] { p });
                            };
                        }
                        Console.WriteLine($"Regsitered method to process command type: {pParameterType}");
                    }
                }
            }
        }

        private static void RegisterCommandsEventsForWorker(params Type[] allTypes)
        {
            var listCmds = allTypes.Where(t => typeof(ICommand).IsAssignableFrom(t)
                         && t.IsClass && !t.IsAbstract).ToList();
            var listEvts = allTypes.Where(t => typeof(IEvent).IsAssignableFrom(t)
              && t.IsClass && !t.IsAbstract).ToList();

            if (listCmds.Count > 0 || listEvts.Count > 0)
            {
                lock (_commandLocker)
                {
                    foreach (var cmd in listCmds)
                    {
                        _commandsEvents[cmd.FullName.ToLower()] = cmd;
                    }
                }

                lock (_eventLocker)
                {
                    foreach (var evt in listEvts)
                    {
                        _commandsEvents[evt.FullName.ToLower()] = evt;
                    }
                }
            }
        }

        public static Type FindTypeOfCommandOrEvent(string fullNameOrName)
        {
            Type type = null;
            fullNameOrName = fullNameOrName.ToLower();
            if (_commandsEvents.TryGetValue(fullNameOrName, out type))
            {
                return type;
            }

            return null;
        }

        public static void RegisterEvent<T>(Action<T> handle, string subscriberName) where T : IEvent
        {
            var t = typeof(T).FullName;
            lock (_eventLocker)
            {

                List<Action<IEvent>> ax;

                if (_eventHandler.TryGetValue(t, out ax))
                {
                    ax.Add(p => handle((T)p));
                }
                else
                {
                    ax = new List<Action<IEvent>>() { p => handle((T)p) };
                }

                _eventHandler[t] = ax;
            }
            RegisterCommandsEventsForWorker(typeof(T));

            EngineeEventWorkerQueue.Subscribe(t, (o) =>
            {
                handle((T)o);
            }, subscriberName);
        }

        internal static void PushEvent(IEvent e, bool execAsync = false)
        {
            if (execAsync)
            {
                EngineeEventWorkerQueue.Push(e);
            }
            else
            {
                ExecEvent(e);
            }
        }

        internal static void ExecEvent(IEvent e)
        {
            var t = e.GetType().FullName;
            List<Action<IEvent>> listAction;
            lock (_eventLocker)
            {
                if (!_eventHandler.TryGetValue(t, out listAction))
                {
                    throw new EntryPointNotFoundException($"Not found type: {t}. Check {nameof(CommandsAndEventsRegisterEngine)} or {nameof(CommandsAndEventsRegisterEngine.RegisterEvent)}");
                }
            }
            Console.WriteLine("#begin evt: " + e.PublishedEventId + " " + t);
            var i = 0;
            foreach (var a in listAction)
            {
                Console.WriteLine("#begin evt to subscriber: " + i);

                a(e);
                i++;
            }
            Console.WriteLine("#done evt: " + e.PublishedEventId + " " + t);

        }

        public static void RegisterCommand<T>(Action<T> handle) where T : ICommand
        {
            var t = typeof(T).FullName;
            lock (_commandLocker)
            {

                Action<ICommand> ax;

                if (_commandHandler.TryGetValue(t, out ax))
                {
                    return;
                    //throw new Exception($"Should only one handle to cover type: {t}. Check DomainEngine.Boot");
                }

                _commandHandler[t] = (p) => handle((T)p);
            }
            RegisterCommandsEventsForWorker(typeof(T));
        }

        internal static void PushCommand(ICommand c, bool execAsync = false)
        {
            LogCommand(c);

            if (execAsync)
            {
                EngineeCommandWorkerQueue.Push(c);
            }
            else
            {
                ExecCommand(c);
            }
        }

        internal static void ExecCommand(ICommand c)
        {
            var t = c.GetType().FullName;
            Action<ICommand> a;
            lock (_commandLocker)
            {
                if (!_commandHandler.TryGetValue(t, out a))
                {
                    throw new EntryPointNotFoundException($"Not found type: {t}. Check {nameof(CommandsAndEventsRegisterEngine.AutoRegister)} or {nameof(CommandsAndEventsRegisterEngine.RegisterCommand)}");
                }
            }

            try
            {
                Console.WriteLine("#start cmd: " + c.PublishedCommandId + " " + t);
                a(c);
                Console.WriteLine("#done cmd: " + c.PublishedCommandId + " " + t);
                LogCommandState(c, CommandEventStorageState.Done, "Success", null);
            }
            catch (Exception ex)
            {
                Console.WriteLine("#fail cmd: " + c.PublishedCommandId + " " + t);

                LogCommandState(c, CommandEventStorageState.Fail, ex.GetAllMessages(), ex);
                throw ex;
            }

        }

        private static void LogCommand(ICommand c)
        {
            Task.Run(() =>
            {
                using (var db = new CommandEventStorageDbContext())
                {
                    db.CommandEventStorages.Add(new CommandEventStorage()
                    {
                        CreatedDate = DateTime.Now,
                        DataJson = JsonConvert.SerializeObject(c),
                        DataType = c.GetType().FullName,
                        Id = c.PublishedCommandId,
                        IsCommand = true
                    });
                    db.SaveChanges();
                }

            });

            LogCommandState(c, CommandEventStorageState.Pending, "Pending", null);
        }

        private static void LogCommandState(ICommand c, CommandEventStorageState state, string msg, Exception ex)
        {
            Task.Run(() =>
            {
                if (ex != null)
                {
                    msg += "\r\n" + ex.StackTrace;
                }
                using (var db = new CommandEventStorageDbContext())
                {
                    db.CommandEventStorageHistories.Add(new CommandEventStorageHistory()
                    {
                        CommandEventId = c.PublishedCommandId,
                        CreatedDate = DateTime.Now,
                        Id = Guid.NewGuid(),
                        Message = msg,
                        State = (int)state
                    });
                    db.SaveChanges();
                }
            });
        }

        internal static bool CommandWorkerCanDequeue(string type)
        {
            return _commandHandler.ContainsKey(type);
        }

        internal static bool EventWorkerCanDequeue(string type)
        {
            return _eventHandler.ContainsKey(type);
        }
    }
}
