using System.Text.Json;

namespace MoexIntegration.Core.Abstractions
{
    public interface IMoexApiRequest
    {
        Task<JsonElement> GetName(string ticker);
        Task<JsonElement> GetPrice(string ticker);
        Task<JsonElement> GetSharesOutstanding(string ticker);
        Task<JsonElement> GetIsin(string ticker);
    }
}
