using Redis.API.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;
using Redis.API.Configuration;

namespace DemoRedis.API.Attributes
{

    // trước khi nó chạy vào method thì nó sẽ chạy qua cache này <=> cache được gán là attribute
    public class CacheAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int _timeToLiveSeconds;
        public CacheAttribute(int timeToLiveSeconds = 1000) // set time là 1000 nếu như nta k truyền time vào thì nó mặc định là 1000
        {
            _timeToLiveSeconds = timeToLiveSeconds;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // lấy cache ra 
           var cacheConfiguration =     context.HttpContext.RequestServices.GetRequiredService<RedisConfiguration>();
            if (!cacheConfiguration.Enable)  // nếu k có cache
            { 
                await next(); // chạy tiếp xuống repo để lấy dữ liệu 
                return; // vì viết ở đây sau khi nó lấy dữ liệu xong nó sẽ quay lại đây chạy tiếp => return 
            }
            var cacheService = context.HttpContext.RequestServices.GetRequiredService<ICacheService>();
            var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);
            var cacheResonse = await cacheService.GetCacheAsync(cacheKey);

            // nếu trong cache có dữ liệu với key truyền vào thì lấy ra 
            if(!string.IsNullOrEmpty(cacheResonse))
            {
                var contentResult = new ContentResult
                {
                    Content = cacheResonse,
                    ContentType = "application/json",
                    StatusCode = 200,
                };
                context.Result = contentResult;
                return;
            }

            // còn chưa có thì đưa vào cache 
            // b1: Chạy tiếp để lấy dữ liệu
            var exutedContext = await next();
            // sau đó từ dữ liệu vừa lấy tạo ra 1 cache mới vừa lấy 
            //OkObjectResult => trên controller phải trả về dữ liệu Ok() mới lưu được vào cache
            if (exutedContext.Result is OkObjectResult objectResult)
            {
                await cacheService.SetCacheAsync(cacheKey, objectResult.Value, TimeSpan.FromSeconds(_timeToLiveSeconds));
            }
        }
        private static string GenerateCacheKeyFromRequest(HttpRequest request)
        {
            var keyBuider = new StringBuilder();
            keyBuider.Append($"{request.Path}");
            foreach ( var (key , value) in request.Query.OrderBy(x => x.Key) ) 
            {
                keyBuider.Append($"|{key}-{value}");
            }
            return keyBuider.ToString().ToLower();
        }
    }
}
