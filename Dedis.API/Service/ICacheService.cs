
namespace Redis.API.Service
{
    public interface ICacheService
    {
        Task SetCacheAsync(string cacheKey, object reponse, TimeSpan timeout);
        Task<string> GetCacheAsync(string cacheKey);
        Task RemoveCacheAsync(string patern);
    }
}
