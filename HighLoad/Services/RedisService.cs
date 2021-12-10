using System;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Memory;
using StackExchange.Redis;

namespace HighLoad.Services
{
    public interface IMemoryService
    {
        public Task AddAsync<T>(string key, T value);
        public Task<T?> GetAsync<T>(string key);
    }
    public class MemoryService : IMemoryService
    {
        private readonly IMemoryCache _cache;

        public MemoryService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public Task AddAsync<T>(string key, T value)
        {
            _cache.Set(key, value);
            return Task.CompletedTask;
        }

        public Task<T?> GetAsync<T>(string key)
        {
            var value = _cache.Get<T?>(key);
            return Task.FromResult(value);
        }
    }

    public class RedisService: IMemoryService
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;

        public RedisService(IConnectionMultiplexer connectionMultiplexer)
        {
            _connectionMultiplexer = connectionMultiplexer;
        }
 
        public async Task AddAsync<T>(string key, T value)
        {
            var database = _connectionMultiplexer.GetDatabase();
            var jsonsStr = JsonSerializer.Serialize(value);
            await database.StringSetAsync(key, jsonsStr);
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var database = _connectionMultiplexer.GetDatabase();
            var result = await database.StringGetAsync(key);
            if (result.HasValue)
                return JsonSerializer.Deserialize<T>(result.Box() as string ?? throw new InvalidOperationException());
            return default;
        }
    }
}