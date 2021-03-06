﻿using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;

namespace IotHub.Core.Redis
{
    public static class RedisServices
    {
        static IServer _server;
        static SocketManager _socketManager;
        static IConnectionMultiplexer _connectionMultiplexer;
        static ConfigurationOptions _options = null;

        public static bool IsEnable { get; private set; }

        static IMemoryCache _cache;

        static RedisServices()
        {
            _cache = new MemoryCache(new MemoryCacheOptions()
            {
            });
        }

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
            if (_cache == null) _cache = new MemoryCache(new MemoryCacheOptions());

            IsEnable = !string.IsNullOrEmpty(endPoint);

            var soketName = endPoint ?? "127.0.0.1";
            _socketManager = new SocketManager(soketName);

            port = port ?? 6379;

            _options = new ConfigurationOptions()
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
            if (_options == null) throw new Exception($"Must call {nameof(RedisServices.Init)}");
            return ConnectionMultiplexer.Connect(_options);
        }

        public static T Get<T>(string key)
        {
            if (!IsEnable)
            {
                string val1;
                if (_cache.TryGetValue<string>(key, out val1) && !string.IsNullOrEmpty(val1))
                {
                    return JsonConvert.DeserializeObject<T>(val1);
                }
                return default(T);
            }

            var val = RedisDatabase.StringGet(key);
            if (val.HasValue == false) return default(T);

            return JsonConvert.DeserializeObject<T>(val);
        }

        public static void Set<T>(string key, T val, TimeSpan? expireAfter = null)
        {
            if (!IsEnable)
            {
                if (expireAfter != null)
                    _cache.Set<string>(key, JsonConvert.SerializeObject(val), expireAfter.Value);
                else
                    _cache.Set<string>(key, JsonConvert.SerializeObject(val));
                return;
            }

            RedisDatabase.StringSet(key, JsonConvert.SerializeObject(val), expireAfter);
        }

        public static string Get(string key)
        {
            if (!IsEnable)
            {
                string val1;
                if (_cache.TryGetValue<string>(key, out val1) && !string.IsNullOrEmpty(val1))
                {
                    return val1;
                }
                return null;
            }

            var val = RedisDatabase.StringGet(key);
            return val;
        }

        public static void Set(string key, string val, TimeSpan? expireAfter = null)
        {
            if (!IsEnable)
            {
                if (expireAfter != null)
                    _cache.Set<string>(key, val, expireAfter.Value);
                else
                    _cache.Set<string>(key, val);
                return;
            }

            RedisDatabase.StringSet(key, val, expireAfter);
        }
    }
}
