
using System.Globalization;
using System.Text.Json;

namespace MoexIntegration.Core.Domain.Model
{
    /// <summary>
    /// Информация по акции
    /// </summary>
    public class StockInfo
    {
        /// <summary>
        ///     Ctr
        /// </summary>
        private StockInfo()
        {
        }

        /// <summary>
        ///     Ctr
        /// </summary>
        /// <param name="ticker">Тикер</param>
        private StockInfo(string ticker) : this()
        {
            Ticker = ticker;
        }

        /// <summary>
        /// тикер
        /// </summary>
        public string? Ticker { get; private set; }

        /// <summary>
        /// название
        /// </summary>
        public string? Name { get; private set; }

        /// <summary>
        /// ISIN
        /// </summary>
        public string? Isin { get; private set; }

        /// <summary>
        /// текущая цена
        /// </summary>
        public double LastPrice { get; private set; }

        /// <summary>
        /// количество акций в обращении
        /// </summary>
        public long SharesOutstanding { get; private set; }

        /// <summary>
        /// капитализация
        /// </summary>
        public decimal MarketCapitalization { get; private set; }

        /// <summary>
        ///     Factory Method
        /// </summary>
        /// <param name="ticker">Тикер</param>
        /// <returns>Объект StockInfo</returns>
        public static StockInfo Create(string ticker)
        {
            if (String.IsNullOrEmpty(ticker)) throw new ArgumentNullException(nameof(ticker), "тикер не может быть пустым");

            return new StockInfo(ticker);
        }

        /// <summary>
        ///     Парсинг названия компании
        /// </summary>
        /// <param name="data">ответ от Moex в JSON</param>
        /// <returns>Статус</returns>
        public bool AddName(JsonElement data)
        {
            // Проверка наличия секции marketdata
            if (!data.TryGetProperty("securities", out JsonElement securities))
            {
                Console.WriteLine("Cекция 'securities' отсутствует в ответе MOEX");
                return false;
            }

            // Проверка наличия колонок
            if (!securities.TryGetProperty("columns", out JsonElement columnsEl) || columnsEl.ValueKind != JsonValueKind.Array)
            {
                Console.WriteLine("Секция 'columns' отсутствует или неверного типа");
                return false;
            }

            // Проверка наличия данных
            if (!securities.TryGetProperty("data", out JsonElement dataEl) || dataEl.GetArrayLength() == 0)
            {
                Console.WriteLine("Нет данных для тикера " + Ticker);
                return false;
            }

            var columns = columnsEl.EnumerateArray().Select(x => x.GetString()).ToList();
            var row = dataEl[0].EnumerateArray().Select(x => x.ToString()).ToList();

            // Проверка наличия колонки SHORTNAME
            if (!columns.Contains("SHORTNAME"))
            {
                Console.WriteLine("Колонка 'SHORTNAME' отсутствует в ответе");
                return false;
            }

            // парсинг
            Name = Convert.ToString(row[columns.IndexOf("SHORTNAME")]);

            return true;
        }

        /// <summary>
        ///     Парсинг текущей цены
        /// </summary>
        /// <param name="data">ответ от Moex в JSON</param>
        /// <returns>Статус</returns>
        public bool AddLastPrice(JsonElement data)
        {
            double LAST = 0;
            double LCURRENTPRICE = 0;
            double PREVPRICE = 0;

            //Проверка секции marketdata

            // Проверка наличия секции marketdata
            if (!data.TryGetProperty("marketdata", out JsonElement marketdata))
            {
                Console.WriteLine("Cекция 'marketdata' отсутствует в ответе MOEX");
                return false;
            }

            // Проверка наличия колонок
            if (!marketdata.TryGetProperty("columns", out JsonElement marketdataColumnsEl) || marketdataColumnsEl.ValueKind != JsonValueKind.Array)
            {
                Console.WriteLine("Секция 'columns' отсутствует или неверного типа");
                return false;
            }

            // Проверка наличия данных
            if (!marketdata.TryGetProperty("data", out JsonElement marketdataDataEl) || marketdataDataEl.GetArrayLength() == 0)
            {
                Console.WriteLine("Нет данных для тикера " + Ticker);
                return false;
            }

            var marketdataColumns = marketdataColumnsEl.EnumerateArray().Select(x => x.GetString()).ToList();
            var marketdataRow = marketdataDataEl[0].EnumerateArray().Select(x => x.ToString()).ToList();


            // Проверка наличия колонки LAST
            if (!marketdataColumns.Contains("LAST"))
            {
                Console.WriteLine("Колонка 'LAST' отсутствует в ответе");
                return false;
            }
            // Проверка наличия колонки LCURRENTPRICE
            if (!marketdataColumns.Contains("LCURRENTPRICE"))
            {
                Console.WriteLine("Колонка 'LCURRENTPRICE' отсутствует в ответе");
                return false;
            }

            // парсинг LAST
            if (double.TryParse(marketdataRow[marketdataColumns.IndexOf("LAST")], NumberStyles.Any, CultureInfo.InvariantCulture, out double lastPrice))
            {
                LAST = lastPrice;
            }
            else
            {
                LAST = 0;
            }
            // парсинг LCURRENTPRICE
            if (double.TryParse(marketdataRow[marketdataColumns.IndexOf("LCURRENTPRICE")], NumberStyles.Any, CultureInfo.InvariantCulture, out double lcurrentPrice))
            {
                LCURRENTPRICE = lcurrentPrice;
            }
            else
            {
                LCURRENTPRICE = 0;
            }

            //Проверка секции securities

            // Проверка наличия секции securities
            if (!data.TryGetProperty("securities", out JsonElement securities))
            {
                Console.WriteLine("Cекция 'securities' отсутствует в ответе MOEX");
                return false;
            }

            // Проверка наличия колонок
            if (!securities.TryGetProperty("columns", out JsonElement securitiesColumnsEl) || securitiesColumnsEl.ValueKind != JsonValueKind.Array)
            {
                Console.WriteLine("Секция 'columns' отсутствует или неверного типа");
                return false;
            }

            // Проверка наличия данных
            if (!securities.TryGetProperty("data", out JsonElement securitiesDataEl) || securitiesDataEl.GetArrayLength() == 0)
            {
                Console.WriteLine("Нет данных для тикера " + Ticker);
                return false;
            }

            var securitiesColumns = securitiesColumnsEl.EnumerateArray().Select(x => x.GetString()).ToList();
            var securitiesRow = securitiesDataEl[0].EnumerateArray().Select(x => x.ToString()).ToList();

            // Проверка наличия колонки PREVPRICE
            if (!securitiesColumns.Contains("PREVPRICE"))
            {
                Console.WriteLine("Колонка 'PREVPRICE' отсутствует в ответе");
                return false;
            }

            // парсинг PREVPRICE
            if (double.TryParse(securitiesRow[securitiesColumns.IndexOf("PREVPRICE")], NumberStyles.Any, CultureInfo.InvariantCulture, out double prevPrice))
            {
                PREVPRICE = prevPrice;
            }
            else
            {
                PREVPRICE = 0;
            }

            if (LAST != 0) LastPrice = LAST;
            else if (LCURRENTPRICE != 0) LastPrice = LCURRENTPRICE;
            else LastPrice = PREVPRICE;

            return true;
        }

        /// <summary>
        ///     Парсинг количества акций в обращении
        /// </summary>
        /// <param name="data">ответ от Moex в JSON</param>
        /// <returns>Статус</returns>
        public bool AddSharesOutstanding(JsonElement data)
        {
            // Проверка наличия секции securities
            if (!data.TryGetProperty("securities", out JsonElement securities))
            {
                Console.WriteLine("Cекция 'securities' отсутствует в ответе MOEX");
                return false;
            }

            // Проверка наличия колонок
            if (!securities.TryGetProperty("columns", out JsonElement columnsEl) || columnsEl.ValueKind != JsonValueKind.Array)
            {
                Console.WriteLine("Секция 'columns' отсутствует или неверного типа");
                return false;
            }

            // Проверка наличия данных
            if (!securities.TryGetProperty("data", out JsonElement dataEl) || dataEl.GetArrayLength() == 0)
            {
                Console.WriteLine("Нет данных для тикера " + Ticker);
                return false;
            }

            var columns = columnsEl.EnumerateArray().Select(x => x.GetString()).ToList();
            var row = dataEl[0].EnumerateArray().Select(x => x.ToString()).ToList();

            // Проверка наличия колонки ISSUESIZE
            if (!columns.Contains("ISSUESIZE"))
            {
                Console.WriteLine("Колонка 'ISSUESIZE' отсутствует в ответе");
                return false;
            }

            // Парсинг
            if (long.TryParse(row[columns.IndexOf("ISSUESIZE")], NumberStyles.Any, CultureInfo.InvariantCulture, out long sharesOutstanding))
            {
                SharesOutstanding = sharesOutstanding;
            }
            else
            {
                Console.WriteLine("Не удалось распарсить значение ISSUESIZE в число (тип long)");
            }

            return true;
        }

        /// <summary>
        ///     Парсинг ISIN
        /// </summary>
        /// <param name="data">ответ от Moex в JSON</param>
        /// <returns>Статус</returns>
        public bool AddIsin(JsonElement data)
        {
            // Проверка наличия секции securities
            if (!data.TryGetProperty("securities", out JsonElement securities))
            {
                Console.WriteLine("Cекция 'securities' отсутствует в ответе MOEX");
                return false;
            }

            // Проверка наличия колонок
            if (!securities.TryGetProperty("columns", out JsonElement columnsEl) || columnsEl.ValueKind != JsonValueKind.Array)
            {
                Console.WriteLine("Секция 'columns' отсутствует или неверного типа");
                return false;
            }

            // Проверка наличия данных
            if (!securities.TryGetProperty("data", out JsonElement dataEl) || dataEl.GetArrayLength() == 0)
            {
                Console.WriteLine("Нет данных для тикера " + Ticker);
                return false;
            }

            var columns = columnsEl.EnumerateArray().Select(x => x.GetString()).ToList();
            var row = dataEl[0].EnumerateArray().Select(x => x.ToString()).ToList();

            // Проверка наличия колонки ISIN
            if (!columns.Contains("ISIN"))
            {
                Console.WriteLine("Колонка 'ISIN' отсутствует в ответе");
                return false;
            }

            // Парсинг
            Isin = Convert.ToString(row[columns.IndexOf("ISIN")]);

            return true;
        }

        /// <summary>
        ///     Рассчет капитализации компании
        /// </summary>
        /// <returns>Статус</returns>
        public bool DefineMarketCapitalization ()
        {
            if (SharesOutstanding <= 0)
            {
                Console.WriteLine($"{Ticker} | Невозможно рассчитать MarketCapitalization: SharesOutstanding <= 0");
                return false;
            }
            if (LastPrice <= 0)
            {
                Console.WriteLine($"{Ticker} | Невозможно рассчитать MarketCapitalization: LastPrice <= 0");
                return false;
            }

            MarketCapitalization = (decimal)LastPrice * SharesOutstanding;
            return true;
        }
    }
}
