using MoexIntegration.Core.Domain.Model.Securities;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace MoexIntegration.Core.Domain.Model.Prices
{
    /// <summary>
    /// История цен по одному тикеру
    /// </summary>
    public class TickerPriceHistory
    {
        private TickerPriceHistory()
        {
            Prices = new List<PricePoint>();
        }

        private TickerPriceHistory(string ticker) : this()
        {
            Ticker = ticker;
        }

        public string? Ticker { get; private set; }

        public List<PricePoint> Prices { get; private set; }

        public static TickerPriceHistory Create(string ticker)
        {
            if (string.IsNullOrWhiteSpace(ticker)) throw new ArgumentNullException(nameof(ticker), "тикер не может быть пустым");

            return new TickerPriceHistory(ticker);
        }

        public void AddPricePoint(DateTime time, decimal close)
        {
            Prices.Add(PricePoint.Create(time, close));
        }

        public bool AddPrice(JsonElement root)
        {
            // Проверка наличия секции candles
            if (!root.TryGetProperty("candles", out JsonElement candles))
            {
                Console.WriteLine($"Ticker: {Ticker} | Cекция 'candles' отсутствует в ответе MOEX");
                return false;
            }

            // Проверка наличия колонок
            if (!candles.TryGetProperty("columns", out JsonElement columnsEl) || columnsEl.ValueKind != JsonValueKind.Array)
            {
                Console.WriteLine($"Ticker: {Ticker} | Секция 'columns' отсутствует или неверного типа");
                return false;
            }

            // Проверка наличия данных
            if (!candles.TryGetProperty("data", out JsonElement dataEl) || dataEl.GetArrayLength() == 0)
            {
                Console.WriteLine($"Ticker: {Ticker} | Секция 'data' отсутствует или неверного типа");
                return false;
            }

            var columns = columnsEl.EnumerateArray().Select(x => x.GetString()).ToList();

            // Проверка наличия колонки begin (время открытия свечи)
            if (!columns.Contains("begin"))
            {
                Console.WriteLine($"Ticker: {Ticker} | Колонка 'begin' отсутствует в ответе");
                return false;
            }
            // Проверка наличия колонки close (цена закрытия свечи)
            if (!columns.Contains("close"))
            {
                Console.WriteLine($"Ticker: {Ticker} | Колонка 'close' отсутствует в ответе");
                return false;
            }

            // парсинг
            var beginIndex = columns.IndexOf("begin");
            var closeIndex = columns.IndexOf("close");

            foreach (var rowEl in dataEl.EnumerateArray())
            {
                if (rowEl.ValueKind != JsonValueKind.Array) continue;

                var cells = rowEl.EnumerateArray().ToList();

                var beginRaw = cells[beginIndex].ToString();
                var closeRaw = cells[closeIndex].ToString();

                if (string.IsNullOrWhiteSpace(beginRaw)) continue;
                if (string.IsNullOrWhiteSpace(closeRaw)) continue;

                if (!DateTime.TryParse(beginRaw, out var time)) continue;
                if (!decimal.TryParse(closeRaw, NumberStyles.Any, CultureInfo.InvariantCulture, out var close)) continue;

                Prices.Add(PricePoint.Create(time, close));
            }

            return true;
        }
    }
}
