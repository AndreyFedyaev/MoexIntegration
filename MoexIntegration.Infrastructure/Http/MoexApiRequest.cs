using MoexIntegration.Core.Abstractions;
using System.Text.Json;

namespace MoexIntegration.Infrastructure.Http
{
    public class MoexApiRequest : IMoexApiRequest
    {
        public async Task<JsonElement> GetPrice (string ticker)
        {
            using var client = new HttpClient();

            string urlPrice = $"https://iss.moex.com/iss/engines/stock/markets/shares/boards/TQBR/securities/{ticker}.json?iss.only=marketdata&marketdata.columns=SECID,LAST";
            var jsonPrice = await client.GetStringAsync(urlPrice);
            var docPrice = JsonDocument.Parse(jsonPrice);
            var marketdata = docPrice.RootElement.GetProperty("marketdata");

            return marketdata;
        }
    }
}
