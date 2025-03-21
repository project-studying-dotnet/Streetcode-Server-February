namespace Streetcode.BLL.Services.CacheService;

public class RedisEnviromentVariables
{
    required public int ExpirationTimeInSeconds { get; set; }

    required public bool Enabled { get; set; }
}
