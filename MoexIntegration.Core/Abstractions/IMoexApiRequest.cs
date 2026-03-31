using System.Text.Json;

namespace MoexIntegration.Core.Abstractions
{
    public interface IMoexApiRequest
    {
        Task<JsonElement> GetName(string ticker);
        Task<JsonElement> GetPrice(string ticker);
        Task<JsonElement> GetCandleHistoryDay(string ticker, int interval = 10);
        Task<JsonElement> GetCandleHistoryWeek(string ticker, int interval = 10);
        Task<JsonElement> GetCandleHistoryMonth(string ticker, int interval = 10);
        Task<JsonElement> GetCandleHistoryYear(string ticker, int interval = 10);
        Task<JsonElement> GetCandleHistory(string ticker, DateTime from, DateTime till, int interval = 10);
        Task<JsonElement> GetSharesOutstanding(string ticker);
        Task<JsonElement> GetIsin(string ticker);
        Task<JsonElement> GetAllSecurities();
    }
}
