namespace Redis.API.Configuration
{
    public class RedisConfiguration // đây chính là biến để binding dữ liệu với file appsetting.json 
    {
        // sử dụng tên giống nhau để binding dễ hơn , không phải gán lại nữa 
        public bool Enable { get; set; }
        public string ConnectionString { get; set; }
    }
}
