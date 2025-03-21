using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;
using Streetcode.BLL.Interfaces.CacheService;
using Streetcode.BLL.Services.Payment;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Streetcode.BLL.Services.CacheService;

public class RedisCacheService : IRedisCacheService
{
    private readonly IConnectionMultiplexer _redis;

    private readonly TimeSpan _cacheExpiration;

    public RedisCacheService(IConnectionMultiplexer redis, IOptions<RedisEnviromentVariables> redisEnviromentVariables)
    {
        _redis = redis;

        _cacheExpiration = TimeSpan.FromSeconds(redisEnviromentVariables.Value.ExpirationTimeInSeconds);
    }

    public async Task SetCacheValueAsync<T>(string key, T? value)
    {
        if (value is null)
        {
            return;
        }

        var db = _redis.GetDatabase();
        var json = JsonConvert.SerializeObject(value, new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        });

        await db.StringSetAsync(key, json, _cacheExpiration);
    }

    public async Task<T?> GetCacheValueAsync<T>(string key)
    {
        var db = _redis.GetDatabase();
        var json = await db.StringGetAsync(key);

        return json.HasValue ? JsonSerializer.Deserialize<T>(json!) : default;
    }
}
