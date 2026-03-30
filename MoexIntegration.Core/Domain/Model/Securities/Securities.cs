using System.Text.Json;

namespace MoexIntegration.Core.Domain.Model.Securities
{
    public class Securities
    {
        /// <summary>
        ///     Ctr
        /// </summary>
        private Securities()
        {
            SecurityList = new List<Security>();
        }

        /// <summary>
        /// Список всех активов РФ (тикеров)
        /// </summary>
        public List<Security> SecurityList { get; private set; }

        /// <summary>
        /// Список групп активов
        /// </summary>
        public List<SecurityGroup> SecurityGroups { get; private set; }

        /// <summary>
        ///     Factory Method
        /// </summary>
        /// <param name="ticker">Тикер</param>
        /// <returns>Объект StockInfo</returns>
        public static Securities Create()
        {

            return new Securities();
        }

        /// <summary>
        ///     Создание списка активов (парсинг данных)
        /// </summary>
        /// <param name="root">ответ от Moex в JSON</param>
        /// <returns>Статус</returns>
        public bool CreateSecurities(JsonElement root)
        {
            // Проверка наличия секции marketdata
            if (!root.TryGetProperty("securities", out JsonElement securities))
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
                Console.WriteLine("Секция 'data' отсутствует или неверного типа");
                return false;
            }

            var columns = columnsEl.EnumerateArray().Select(x => x.GetString()).ToList();

            // Проверка наличия колонки SECID
            if (!columns.Contains("SECID"))
            {
                Console.WriteLine("Колонка 'SECID' отсутствует в ответе");
                return false;
            }
            // Проверка наличия колонки SHORTNAME
            if (!columns.Contains("SHORTNAME"))
            {
                Console.WriteLine("Колонка 'SHORTNAME' отсутствует в ответе");
                return false;
            }
            // Проверка наличия колонки ISIN
            if (!columns.Contains("ISIN"))
            {
                Console.WriteLine("Колонка 'ISIN' отсутствует в ответе");
                return false;
            }

            // парсинг
            foreach (var rowEl in dataEl.EnumerateArray())
            {
                if (rowEl.ValueKind != JsonValueKind.Array) continue;

                var cells = rowEl.EnumerateArray().ToList();

                if (string.IsNullOrWhiteSpace(cells[columns.IndexOf("SECID")].GetString())) continue;

                SecurityList.Add(new Security
                {
                    Ticker = cells[columns.IndexOf("SECID")].GetString() ?? "",
                    ShortName = cells[columns.IndexOf("SHORTNAME")].GetString() ?? "",
                    ISIN = cells[columns.IndexOf("ISIN")].GetString() ?? ""
                });
            }

            return true;
        }

        /// <summary>
        ///     Создание групп активов
        /// </summary>
        /// <returns>Статус</returns>
        public bool CreateGroups()
        {
            bool result = false;
            try
            {
                int securityCounter = 0;
                int groupIndex = 0;

                SecurityGroups =
                [
                    new SecurityGroup
                    {
                        GroupeName = "Group" + (groupIndex + 1),
                        SecurityList = []
                    },
                ];

                Console.WriteLine($"Сгенерирована группа активов: {SecurityGroups[groupIndex].GroupeName}");

                foreach (var security in SecurityList)
                {
                    if (securityCounter >= 20)
                    {
                        groupIndex++;

                        SecurityGroups.Add(new SecurityGroup
                        {
                            GroupeName = "Group" + (groupIndex + 1),
                            SecurityList = []
                        });

                        Console.WriteLine($"Сгенерирована группа активов: {SecurityGroups[groupIndex].GroupeName}");

                        securityCounter = 0;
                    }

                    SecurityGroups[groupIndex].SecurityList.Add(security);
                    securityCounter++;

                }

                Console.WriteLine($"Сгенерировано всего: {SecurityGroups.Count} групп...");

                result = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception | Ошибка выполнения метода CreateGroups в классе Securities: {ex.Message}");
            }

            return result;
        }
    }
}
