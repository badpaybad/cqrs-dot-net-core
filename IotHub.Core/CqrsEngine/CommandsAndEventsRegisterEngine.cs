using IotHub.Core.Cqrs;
using IotHub.Core.Reflection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace IotHub.Core.CqrsEngine
{
    public static class CommandsAndEventsRegisterEngine
    {
        static readonly Dictionary<Type, List<Action<IEvent>>> _eventHandler = new Dictionary<Type, List<Action<IEvent>>>();
        static readonly Dictionary<Type, Action<ICommand>> _commandHandler = new Dictionary<Type, Action<ICommand>>();

        static object _eventLocker = new object();
        static object _commandLocker = new object();

        static List<string> _commands = new List<string>();
        static List<string> _events = new List<string>();

        static string _connectionString;

        static CommandsAndEventsRegisterEngine()
        {

        }

        public static void Init(string commandEventStorageConnectionString)
        {
            _connectionString = commandEventStorageConnectionString;
        }

        public static bool AutoRegister()
        {
            lock (_commandLocker)
            {
                _commandHandler.Clear();
            }
            lock (_commands)
            {
                _commands.Clear();
            }
            lock (_eventLocker)
            {
                _eventHandler.Clear();
            }
            lock (_events)
            {
                _events.Clear();
            }

            var allAss = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in allAss)
            {
                RegisterAssembly(assembly);
            }

            return true;
        }

        public static void RegisterAssembly(Assembly executingAssembly)
        {
            var allTypes = executingAssembly.GetTypes();
            var listHandler = allTypes.Where(t => typeof(ICqrsHandle).IsAssignableFrom(t)
                                                  && t.IsClass && !t.IsAbstract).ToList();

            var assemblyFullName = executingAssembly.FullName;

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

                    var className = mi.DeclaringType.FullName;
                    if (typeof(IEvent).IsAssignableFrom(pParameterType))
                    {
                        lock (_eventLocker)
                        {
                            var t = pParameterType;

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
                        Console.WriteLine($"Regsitered method to process event type: {pParameterType}");

                        lock (_events)
                        {
                            if (mi.DeclaringType != null)
                            {
                                _events.Add(pParameterType.FullName +
                                              $" [{className}][{assemblyFullName}]");
                            }
                            else
                            {
                                _events.Add(pParameterType.FullName +
                                            $" [{assemblyFullName}]");
                            }
                        }
                    }

                    if (typeof(ICommand).IsAssignableFrom(pParameterType))
                    {
                        lock (_commandLocker)
                        {
                            var t = pParameterType;

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
                        lock (_commands)
                        {
                            if (mi.DeclaringType != null)
                            {
                                _commands.Add(pParameterType.FullName +
                                                $" [{className}][{assemblyFullName}]");
                            }
                            else
                            {
                                _commands.Add(pParameterType.FullName +
                                              $" [{assemblyFullName}]");
                            }
                        }
                    }
                }
            }
        }

        public static void RegisterEvent<T>(Action<T> handle) where T : IEvent
        {
            var t = typeof(T);
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

            lock (_events)
            {
                _events.Add(t.FullName);
            }
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
            var t = e.GetType();
            List<Action<IEvent>> listAction;
            lock (_eventLocker)
            {
                if (!_eventHandler.TryGetValue(t, out listAction))
                {
                    throw new EntryPointNotFoundException($"Not found type: {t}");
                }
            }

            foreach (var a in listAction)
            {
                a(e);
            }
        }

        public static void RegisterCommand<T>(Action<T> handle) where T : ICommand
        {
            var t = typeof(T);
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

            lock (_commands)
            {
                _commands.Add(t.FullName);
            }
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
            var t = c.GetType();
            Action<ICommand> a;
            lock (_commandLocker)
            {
                if (!_commandHandler.TryGetValue(t, out a))
                {
                    throw new EntryPointNotFoundException($"Not found type: {t}. Check DomainEngine.Boot");
                }
            }
           
            try {
                a(c);
                LogCommandState(c, CommandEventStorageState.Done, "Success", null);
            } catch (Exception ex){
                LogCommandState(c, CommandEventStorageState.Fail, ex.GetMessages(),ex);
            }
          
        }


        public static List<string> GetEvents()
        {
            lock (_events)
            {
                return _events;
            }
        }
        public static List<string> GetCommands()
        {
            lock (_commands)
            {
                return _commands;
            }
        }
        
        private static void LogCommand(ICommand c)
        {
            ThreadPool.QueueUserWorkItem((o)=> {
                using (var db = new CommandEventStorageDbContext(_connectionString))
                {
                    db.CommandEventStorages.Add(new CommandEventStorage()
                    {
                        CreatedDate = DateTime.Now,
                        DataJson = JsonConvert.SerializeObject(c),
                        DataType = c.GetType().FullName,
                        Id = c.CommandId,
                        IsCommand = true
                    });
                    db.SaveChanges();
                }

            });
           
            LogCommandState(c, CommandEventStorageState.Pending, "Pending", null);
        }

        private static void LogCommandState(ICommand c, CommandEventStorageState state, string msg, Exception ex)
        {
            ThreadPool.QueueUserWorkItem((o) => {
                if (ex != null)
                {
                    msg += "\r\n" + ex.StackTrace;
                }
                using (var db = new CommandEventStorageDbContext(_connectionString))
                {
                    db.CommandEventStorageHistories.Add(new CommandEventStorageHistory()
                    {
                        CommandEventId = c.CommandId,
                        CreatedDate = DateTime.Now,
                        Id = Guid.NewGuid(),
                        Message = msg,
                        State = (int)state
                    });
                    db.SaveChanges();
                }
            });          
        }              
      
    }
}
