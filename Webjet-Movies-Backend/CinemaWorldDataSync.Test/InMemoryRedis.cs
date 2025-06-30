using RedisLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CinemaWorldDataSync.Test
{
    public class InMemoryRedis : IRedisService
    {
        private readonly Dictionary<string, string> _store = new();

        public async Task SetAsync<T>(string key, T value)
        {
            var jsonValue = JsonSerializer.Serialize(value);

            _store[key] = jsonValue;
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            _store.TryGetValue(key, out var value);

            return JsonSerializer.Deserialize<T>(value);
        }
    }

}
