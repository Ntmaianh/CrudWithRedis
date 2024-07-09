using Redis.API.Configuration;
using Redis.API.Service;
using Redis.DAL;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
var redisConfiguration = new RedisConfiguration();
// binding dữ liệu 
builder.Configuration.GetSection("RedisConfiguration").Bind(redisConfiguration);
builder.Services.AddSingleton(redisConfiguration);

// kiểm tra redis được bật hay chưa ? 
if (!redisConfiguration.Enable)
    return;
builder.Services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisConfiguration.ConnectionString));
builder.Services.AddStackExchangeRedisCache(option => option.Configuration = redisConfiguration.ConnectionString);
builder.Services.AddSingleton<ICacheService, CacheService>();
builder.Services.AddSingleton<IRepository, Repository>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
