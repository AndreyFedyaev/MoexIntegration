using MoexIntegration.Core.Domain.Model.Prices;
using MoexIntegration.Core.Domain.Model.Securities;

namespace MoexIntegration.Core.Abstractions
{
    public interface ICacheService
    {
        Task WriteSecurities(List<Security> data);

        Task WriteGroupSecurity(string groupName, List<Security> data);
        Task WriteGroupPrices(string groupName, string key, TickerPriceHistories data);

        Task<List<Security>> GetGroupSecurities(string groupName);
        Task<List<string>> GetGroups();
    }
}
