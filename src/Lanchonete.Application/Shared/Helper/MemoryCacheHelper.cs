using Microsoft.Extensions.Caching.Memory;

namespace Lanchonete.Application.Shared.Helper;

public class MemoryCacheHelper
{
    private readonly IMemoryCache _cache;

    public MemoryCacheHelper(IMemoryCache cache)
        => _cache = cache;

    public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
    {
        if (_cache.TryGetValue(key, out T? value) && value is not null)
            return value;

        var result = await factory();

        _cache.Set(key, result, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(30)
        });

        return result;
    }

    public void Remove(string key)
    {
        _cache.Remove(key);
    }
}