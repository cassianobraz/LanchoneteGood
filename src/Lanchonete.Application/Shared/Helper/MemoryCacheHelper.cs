using Lanchonete.Domain.Shared.interfaces;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;

namespace Lanchonete.Application.Shared.Helper;

public class MemoryCacheHelper : IMemoryCacheHelper
{
    private readonly IMemoryCache _cache;

    private static readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();

    public MemoryCacheHelper(IMemoryCache cache)
        => _cache = cache;

    public async Task<T> GetOrCreateAsync<T>(
        string key,
        Func<Task<T>> factory,
        TimeSpan? expiration = null)
    {
        if (_cache.TryGetValue(key, out T? value) && value is not null)
            return value;

        var myLock = _locks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));

        await myLock.WaitAsync();

        try
        {
            if (_cache.TryGetValue(key, out value) && value is not null)
                return value;

            var result = await factory();

            _cache.Set(key, result, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(30)
            });

            return result;
        }
        finally
        {
            myLock.Release();
        }
    }

    public void Remove(string key)
    {
        _cache.Remove(key);
    }
}