using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace IotHub.Core.Redis
{
    public static class RedisServices
    {
        static IServer _server;
        static SocketManager _socketManager;
        static IConnectionMultiplexer _connectionMultiplexer;
        static ConfigurationOptions _options = null;

        public static bool IsEnable { get; private set; }

        static IConnectionMultiplexer RedisConnectionMultiplexer
        {
            get
            {
                if (_connectionMultiplexer != null && _connectionMultiplexer.IsConnected)
                    return _connectionMultiplexer;

                if (_connectionMultiplexer != null && !_connectionMultiplexer.IsConnected)
                {
                    _connectionMultiplexer.Dispose();
                }

                _connectionMultiplexer = GetConnection();
                if (!_connectionMultiplexer.IsConnected)
                {
                    var exception = new Exception("Can not connect to redis");
                    Console.WriteLine(exception);
                    throw exception;
                }
                return _connectionMultiplexer;
            }
        }

        public static IDatabase RedisDatabase
        {
            get
            {
                var redisDatabase = RedisConnectionMultiplexer.GetDatabase();

                return redisDatabase;
            }
        }

        public static ISubscriber RedisSubscriber
        {
            get
            {
                var redisSubscriber = RedisConnectionMultiplexer.GetSubscriber();

                return redisSubscriber;
            }
        }

        public static void Init(string endPoint, int? port, string pwd)
        {

            IsEnable = !string.IsNullOrEmpty(endPoint);

            var soketName = endPoint ?? "127.0.0.1";
            _socketManager = new SocketManager(soketName);

            port = port ?? 6379;

            _options = new ConfigurationOptions
            {
                EndPoints =
                {
                    {endPoint, port.Value}
                },
                Password = pwd,
                AllowAdmin = false,
                SyncTimeout = 5 * 1000,
                SocketManager = _socketManager,
                AbortOnConnectFail = false,
                ConnectTimeout = 5 * 1000,
            };

        }

        static ConnectionMultiplexer GetConnection()
        {

            return ConnectionMultiplexer.Connect(_options);
        }
    }
}
