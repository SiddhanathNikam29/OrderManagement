using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using OrderManagement.Domain.Interfaces;
using System.Text.Json;

namespace OrderManagement.Infrastructure.Services
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<RedisCacheService> _logger;

        public RedisCacheService(IDistributedCache cache, ILogger<RedisCacheService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                var value = await _cache.GetStringAsync(key, cancellationToken);
                if (string.IsNullOrEmpty(value))
                    return default;

                return JsonSerializer.Deserialize<T>(value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get cache for key {Key}", key);
                return default;
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var options = new DistributedCacheEntryOptions();
                if (expiry.HasValue)
                    options.AbsoluteExpirationRelativeToNow = expiry;
                else
                    options.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);

                var json = JsonSerializer.Serialize(value);
                await _cache.SetStringAsync(key, json, options, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to set cache for key {Key}", key);
            }
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                await _cache.RemoveAsync(key, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove cache for key {Key}", key);
            }
        }

        public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                var value = await _cache.GetStringAsync(key, cancellationToken);
                return !string.IsNullOrEmpty(value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to check cache existence for key {Key}", key);
                return false;
            }
        }
    }
}