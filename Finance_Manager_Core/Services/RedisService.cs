using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StackExchange.Redis;
using static ServiceStack.Diagnostics.Events;

namespace Finance_Manager_Core.Services
{


    public class RedisService
    {
        private readonly IDatabase _db;
        private readonly IConnectionMultiplexer _redis;

        public RedisService(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
            _redis = redis;
        }

        public async Task SetAsync(string key, string value, int expirySeconds)
        {
            await _db.StringSetAsync(key, value, TimeSpan.FromSeconds(expirySeconds));
        }

        public async Task SetAsync<T>(string key, T value, int expirySeconds)
        {
            var json = JsonConvert.SerializeObject(value);
            await _db.StringSetAsync(key, json, TimeSpan.FromSeconds(expirySeconds));
        }

        public async Task<string?> GetAsync(string key)
        {
            return await _db.StringGetAsync(key);
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var value = await _db.StringGetAsync(key);

            if (value.IsNullOrEmpty)
                return default;

            return JsonConvert.DeserializeObject<T>(value!);
        }

        public async Task RemoveAsync(string key)
        {
            await _db.KeyDeleteAsync(key);
        }

        public async Task<List<string>> GetKeysAsync(string pattern)
        {
            var server = _redis.GetServer(_redis.GetEndPoints().First());
            return server.Keys(pattern: pattern).Select(k => k.ToString()).ToList();
        }
    }
}
