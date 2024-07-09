
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StackExchange.Redis;

namespace Redis.API.Service
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IConnectionMultiplexer _connectionMultiplexer;

        public CacheService(IDistributedCache distributedCache,IConnectionMultiplexer connectionMultiplexer)
        {
            _distributedCache = distributedCache;
            _connectionMultiplexer = connectionMultiplexer;
        }
        public async Task<string> GetCacheAsync(string cacheKey)
        {
            var cacheResponse = await _distributedCache.GetStringAsync(cacheKey);
            return string.IsNullOrEmpty(cacheResponse) ? null : cacheResponse;
        }


        public async Task SetCacheAsync(string cacheKey, object reponse, TimeSpan timeout)
        {
            if (reponse == null)
                return;
            var sezializerResponse = JsonConvert.SerializeObject(reponse,new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver() // vì khi trả dữ liệu về thì dữ liệu thường viết thường hết. Dòng 30 để chuyển đổi dữ liệu theo kiểu camelCase 
            });
            await _distributedCache.SetStringAsync(cacheKey, sezializerResponse, new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = timeout // Nó chỉ định thời điểm tuyệt đối (absolute) mà mục nhập cache sẽ hết hạn
            });
        }


        public async Task RemoveCacheAsync(string patern)
        {
            if (string.IsNullOrEmpty(patern))
            {
                throw new ArgumentNullException("Value is null");
            }
            await foreach(var key in GetKeyAsync(patern))
            {
                await _distributedCache.RemoveAsync(key);
            }
        }

        private async IAsyncEnumerable<string> GetKeyAsync(string patern)
        {
            if (string.IsNullOrEmpty(patern))
            {
                throw new ArgumentNullException("Value is null");
            }
            foreach (var endPoint in _connectionMultiplexer.GetEndPoints()) // lấy ra cái địa chỉ của cache 
            {
                var server = _connectionMultiplexer.GetServer(endPoint);
                foreach (var key in server.Keys(pattern: patern)) // pattern là tham số của key đại diện cho mỗi pattern để lọc từ khóa 
                {
                    //  yield return : dừng vòng lặp hiện tại lại xong chạy vòng lặp khác chứ k return hẳn 
                    yield return key.ToString();
                }
            }
        }
    }
}
