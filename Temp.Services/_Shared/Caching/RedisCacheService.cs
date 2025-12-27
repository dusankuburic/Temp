using Microsoft.Extensions.Logging;

namespace Temp.Services.Caching;

public class RedisCacheService : ICacheService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _database;
    private readonly ILogger<RedisCacheService>? _logger;

    public RedisCacheService(IConnectionMultiplexer redis, ILogger<RedisCacheService>? logger = null) {
        _redis = redis;
        _database = redis.GetDatabase();
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) {
        try {
            var value = await _database.StringGetAsync(key);

            if (value.IsNullOrEmpty)
                return default;

            return JsonConvert.DeserializeObject<T>(value!);
        } catch (RedisConnectionException ex) {
            _logger?.LogWarning(ex, "Redis connection failed for GetAsync with key: {Key}", key);
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) {
        try {
            var serializedValue = JsonConvert.SerializeObject(value);
            await _database.StringSetAsync(key, serializedValue, expiration);
        } catch (RedisConnectionException ex) {
            _logger?.LogWarning(ex, "Redis connection failed for SetAsync with key: {Key}", key);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default) {
        try {
            await _database.KeyDeleteAsync(key);
        } catch (RedisConnectionException ex) {
            _logger?.LogWarning(ex, "Redis connection failed for RemoveAsync with key: {Key}", key);
        }
    }

    public async Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default) {
        try {
            var endpoints = _redis.GetEndPoints();
            var server = _redis.GetServer(endpoints.First());


            var keys = new List<RedisKey>();
            await foreach (var key in server.KeysAsync(pattern: pattern)) {
                keys.Add(key);
            }

            if (keys.Count > 0) {
                await _database.KeyDeleteAsync(keys.ToArray());
            }
        } catch (RedisConnectionException ex) {
            _logger?.LogWarning(ex, "Redis connection failed for RemoveByPatternAsync with pattern: {Pattern}", pattern);
        }
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default) {
        try {
            return await _database.KeyExistsAsync(key);
        } catch (RedisConnectionException ex) {
            _logger?.LogWarning(ex, "Redis connection failed for ExistsAsync with key: {Key}", key);
            return false;
        }
    }
}