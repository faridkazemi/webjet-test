using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RedisLibrary
{
    public class RedisService: IRedisService
    {
        private IDatabase _database;
        private ILogger<RedisService> _logger;

        public RedisService(IConnectionMultiplexer redisConnection,
            ILogger<RedisService> logger) 
        {
            //ConnectionMultiplexer.Connect("redis:6379,abortConnect=false,connectTimeout=10000,syncTimeout=10000").GetDatabase();//
            _database = redisConnection.GetDatabase();
            _logger = logger;
            //var a = _database.Multiplexer.IsConnected;

            ConnectionMultiplexer? multiplexer = null;

            for (int i = 0; i < 5; i++)
            {
                try
                {
                    multiplexer = ConnectionMultiplexer.Connect("redis:6379,abortConnect=false,connectTimeout=10000,syncTimeout=10000");

                    if (multiplexer.IsConnected)
                    {
                        Console.WriteLine("Redis connected.");
                        break;
                    }

                    Console.WriteLine("Waiting for Redis connection...");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Attempt {i + 1}: Redis not ready yet. Error: {ex.Message}");
                }

                Task.Delay(2000);
            }
        }

        public async Task SetAsync<T>(string key, T value)
        {
            try
            {
                var jsonValue = JsonSerializer.Serialize(value);

                if (!_database.Multiplexer.IsConnected)
                {
                    Console.WriteLine("Waiting for Redis to be ready...");
                    await Task.Delay(2000);
                }

                await _database.StringSetAsync(key, jsonValue, null, When.Always, CommandFlags.None);

            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, $"Faild to set values in redis. key {key}");
                throw;
            }

        }

        public async Task<T?> GetAsync<T>(string key)
        {
            try
            {
                var result = await _database.StringGetAsync(key);

                return result.HasValue ? JsonSerializer.Deserialize<T>(result) : default(T?);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Faild to get values from redis. key {key}");
                throw;
            }
            
        }
    }
}
