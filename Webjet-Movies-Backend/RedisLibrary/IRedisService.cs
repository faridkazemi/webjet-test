namespace RedisLibrary
{
    public interface IRedisService
    {
        public Task<T?> GetAsync<T>(string key);
        public Task SetAsync<T>(string key, T value);
    }
}
