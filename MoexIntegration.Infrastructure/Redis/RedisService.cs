using Microsoft.Extensions.Configuration;
using MoexIntegration.Core.Abstractions;
using StackExchange.Redis;

namespace MoexIntegration.Infrastructure.Redis
{
    public class RedisService : ICacheService
    {
        private readonly IDatabase _db;

        public RedisService(IConfiguration configuration)
        {
            var config = new ConfigurationOptions
            {
                EndPoints = { $"{configuration["Redis:Address"]}:{configuration["Redis:Port"]}" },
                Password = $"{configuration["Redis:Password"]}",
                ConnectRetry = 3,
                ConnectTimeout = 5000,
                AbortOnConnectFail = false,
                KeepAlive = 60,
                DefaultDatabase = 0
            };

            var redis = ConnectionMultiplexer.Connect(config);
            _db = redis.GetDatabase();
        }

        public async Task WriteReadString()
        {
            //// 1. Установка строки
            //await _db.StringSetAsync("demo:name", "TestData");

            //// 2. Получение строки
            //var name = await _db.StringGetAsync("demo:name");
        }
    }
}
