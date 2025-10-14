using Microsoft.Extensions.Configuration;
using MoexIntegration.Core.Abstractions;
using MoexIntegration.Core.Domain.Model.Securities;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Text.Json;

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

        public async Task UpdateSecurity(List<Security> data)
        {
            var serializedArray = JsonSerializer.Serialize(data);
            await _db.HashSetAsync("SwcuritiesList", "Securities", serializedArray);
        }
    }
}
