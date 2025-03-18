namespace Streetcode.BLL.Interfaces.CacheService;

public interface IRedisCacheService
{
    Task SetCacheValueAsync<T>(string key, T? value);
    Task<T?> GetCacheValueAsync<T>(string key);
}
