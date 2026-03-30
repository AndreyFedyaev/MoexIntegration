using MoexIntegration.Core.Abstractions;
using System.Text.Json;

namespace MoexIntegration.Infrastructure.Http
{
    public class MoexApiRequest : IMoexApiRequest
    {
        private static readonly TimeZoneInfo MoscowTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");

        public async Task<JsonElement> GetName(string ticker)
        {
            using var client = new HttpClient();

            string url = $"https://iss.moex.com/iss/engines/stock/markets/shares/boards/TQBR/securities/{ticker}.json?iss.only=securities&securities.columns=SECID,SHORTNAME";
            var json = await client.GetStringAsync(url);
            var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            return root;
        }

        public async Task<JsonElement> GetPrice (string ticker)
        {
            using var client = new HttpClient();

            string url = $"https://iss.moex.com/iss/engines/stock/markets/shares/boards/TQBR/securities/{ticker}.json?iss.only=marketdata,securities&marketdata.columns=SECID,LAST,LCURRENTPRICE&securities.columns=SECID,PREVPRICE&iss.meta=off";
            var json = await client.GetStringAsync(url);
            var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
          
            return root;
        }

        public Task<JsonElement> GetCandleHistoryDay(string ticker, int interval = 10)
        {
            var till = DateTime.UtcNow;
            var from = till.AddDays(-1);
            
            return GetCandleHistory(ticker, from, till, interval);
        }

        public async Task<JsonElement> GetCandleHistory(string ticker, DateTime from, DateTime till, int interval = 10)
        {
            using var client = new HttpClient();

            string fromValue = Uri.EscapeDataString(from.ToString("yyyy-MM-dd HH:mm:ss"));
            string tillValue = Uri.EscapeDataString(till.ToString("yyyy-MM-dd HH:mm:ss"));
            string url = $"https://iss.moex.com/iss/engines/stock/markets/shares/boards/TQBR/securities/{ticker}/candles.json?from={fromValue}&till={tillValue}&interval={interval}&iss.only=candles&iss.meta=off";

            var json = await client.GetStringAsync(url);
            var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            return root;
        }

        public async Task<JsonElement> GetSharesOutstanding(string ticker)
        {
            using var client = new HttpClient();

            string url = $"https://iss.moex.com/iss/engines/stock/markets/shares/boards/TQBR/securities/{ticker}.json?iss.only=securities&securities.columns=SECID,ISSUESIZE";
            var json = await client.GetStringAsync(url);
            var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            return root;
        }

        public async Task<JsonElement> GetIsin(string ticker)
        {
            using var client = new HttpClient();

            string url = $"https://iss.moex.com/iss/engines/stock/markets/shares/boards/TQBR/securities/{ticker}.json?iss.only=securities&securities.columns=SECID,ISIN";
            var json = await client.GetStringAsync(url);
            var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            return root;
        }

        public async Task<JsonElement> GetAllSecurities()
        {
            using var client = new HttpClient();

            string url = "https://iss.moex.com/iss/engines/stock/markets/shares/boards/TQBR/securities.json?iss.only=securities&securities.columns=SECID,SHORTNAME,ISIN";
            var json = await client.GetStringAsync(url);
            var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            return root;
        }
    }
}
