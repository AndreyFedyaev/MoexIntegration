using Microsoft.Extensions.Configuration;
using MoexIntegration.Core.Abstractions;
using MoexIntegration.Core.Domain.Model.Prices;
using MoexIntegration.Core.Domain.Model.Securities;
using StackExchange.Redis;
using System.Text.Json;

namespace MoexIntegration.Infrastructure.Redis
{
    public class RedisService : ICacheService
    {
        private readonly IDatabase _db;

        public RedisService(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        //Записываем в группу список тикеров
        public async Task WriteGroupSecurity(string groupName, List<Security> data)
        {
            var serializedArray = JsonSerializer.Serialize(data);
            await _db.StringSetAsync($"MoexIntegration:Groups:{groupName}:SecuritiesList", serializedArray);

            //обновление индексов (списка групп)
            await _db.SetAddAsync("MoexIntegration:Groups:Index", groupName);
        }

        //Записываем в группу цену за указанный период
        public async Task WriteGroupPrices(string groupName, string key, TickerPriceHistories data)
        {
            var serializedArray = JsonSerializer.Serialize(data);
            await _db.StringSetAsync($"MoexIntegration:Groups:{groupName}:{key}", serializedArray);
        }

        //Получение списка активов указанной группы
        public async Task<List<Security>> GetGroupSecurities(string groupName)
        {
            var getResult = await _db.StringGetAsync($"MoexIntegration:Groups:{groupName}:SecuritiesList");

            if (!getResult.HasValue) return [];

            return JsonSerializer.Deserialize<List<Security>>(getResult.ToString()) ?? [];
        }

        //Получение индексов (списка групп)
        public async Task<List<string>> GetGroups()
        {
            var members = await _db.SetMembersAsync("MoexIntegration:Groups:Index");
            if (members.Length == 0) return [];
            return members.Select(x => x.ToString()).ToList();
        }
    }
}
