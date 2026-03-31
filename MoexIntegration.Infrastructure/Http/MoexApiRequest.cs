using MoexIntegration.Core.Abstractions;
using System.Text.Json;

namespace MoexIntegration.Infrastructure.Http
{
    public class MoexApiRequest : IMoexApiRequest
    {
        private readonly HttpClient _client;

        public MoexApiRequest(HttpClient client)
        {
            _client = client;
        }

        private static readonly TimeZoneInfo MoscowTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");

        public async Task<JsonElement> GetName(string ticker)
        {
            string url = $"iss/engines/stock/markets/shares/boards/TQBR/securities/{ticker}.json?iss.only=securities&securities.columns=SECID,SHORTNAME";
            var json = await _client.GetStringAsync(url);
            var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            return root;
        }

        public async Task<JsonElement> GetPrice (string ticker)
        {
            string url = $"iss/engines/stock/markets/shares/boards/TQBR/securities/{ticker}.json?iss.only=marketdata,securities&marketdata.columns=SECID,LAST,LCURRENTPRICE&securities.columns=SECID,PREVPRICE&iss.meta=off";
            var json = await _client.GetStringAsync(url);
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

        public Task<JsonElement> GetCandleHistoryWeek(string ticker, int interval = 10)
        {
            var till = DateTime.UtcNow;
            var from = till.AddDays(-7);

            return GetCandleHistory(ticker, from, till, interval);
        }

        public Task<JsonElement> GetCandleHistoryMonth(string ticker, int interval = 10)
        {
            var till = DateTime.UtcNow;
            var from = till.AddMonths(-1);

            return GetCandleHistory(ticker, from, till, interval);
        }

        public Task<JsonElement> GetCandleHistoryYear(string ticker, int interval = 10)
        {
            var till = DateTime.UtcNow;
            var from = till.AddYears(-1);

            return GetCandleHistory(ticker, from, till, interval);
        }

        public async Task<JsonElement> GetCandleHistory(string ticker, DateTime from, DateTime till, int interval = 10)
        {
            string fromValue = Uri.EscapeDataString(from.ToString("yyyy-MM-dd HH:mm:ss"));
            string tillValue = Uri.EscapeDataString(till.ToString("yyyy-MM-dd HH:mm:ss"));
            string url = $"iss/engines/stock/markets/shares/boards/TQBR/securities/{ticker}/candles.json?from={fromValue}&till={tillValue}&interval={interval}&iss.only=candles&iss.meta=off";

            var json = await _client.GetStringAsync(url);
            var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            return root;
        }

        public async Task<JsonElement> GetSharesOutstanding(string ticker)
        {
            string url = $"iss/engines/stock/markets/shares/boards/TQBR/securities/{ticker}.json?iss.only=securities&securities.columns=SECID,ISSUESIZE";
            var json = await _client.GetStringAsync(url);
            var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            return root;
        }

        public async Task<JsonElement> GetIsin(string ticker)
        {
            string url = $"iss/engines/stock/markets/shares/boards/TQBR/securities/{ticker}.json?iss.only=securities&securities.columns=SECID,ISIN";
            var json = await _client.GetStringAsync(url);
            var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            return root;
        }

        public async Task<JsonElement> GetAllSecurities()
        {
            string url = "iss/engines/stock/markets/shares/boards/TQBR/securities.json?iss.only=securities&securities.columns=SECID,SHORTNAME,ISIN";
            var json = await _client.GetStringAsync(url);
            var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            return root;
        }
    }
}
