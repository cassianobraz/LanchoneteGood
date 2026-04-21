namespace Lanchonete.Domain.Shared.interfaces;

public interface IMemoryCacheHelper
{
    Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null);
    void Remove(string key);
}
