using System.Text.Json;

namespace MoexIntegration.Core.Abstractions
{
    public interface IMoexApiRequest
    {
          Task<JsonElement> GetPrice(string ticker);
    }
}
