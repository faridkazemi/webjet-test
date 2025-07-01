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
            _database = redisConnection.GetDatabase();

            if (_database.Multiplexer.IsConnected)
            {
                Console.WriteLine("Redis connected...");

            }

            _logger = logger;

            ConnectionMultiplexer? multiplexer = null;
        }

        public async Task SetAsync<T>(string key, T value)
        {
            try
            {
                var jsonValue = JsonSerializer.Serialize(value);

          
                _logger.LogInformation($"Adding CinemaWorldMovies to redis...");

                await _database.StringSetAsync(key, jsonValue, null, When.Always, CommandFlags.None);

            }
            catch (RedisConnectionException ex)
            {
                // We probably don't want to throw exception. Just log and go
                _logger.LogError(ex, $"Faild to connect to redis.");
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
                _logger.LogInformation($"Fetching CinemaWorldMovies from redis. Key: {key} ..." );

                var result = await _database.StringGetAsync(key);

                return result.HasValue ? JsonSerializer.Deserialize<T>(result) : default(T?);
            }
            catch (RedisConnectionException ex)
            {
                // We probably don't want to throw exception. Just log and go
                _logger.LogError(ex, $"Faild to connect to redis.");
                return default(T?);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Faild to get values from redis. key {key}");
                throw;
            }
            
        }
    }
}
