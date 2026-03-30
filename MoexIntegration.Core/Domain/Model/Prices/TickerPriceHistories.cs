namespace MoexIntegration.Core.Domain.Model.Prices
{
    /// <summary>
    /// История цен по всем тикерам
    /// </summary>
    public class TickerPriceHistories
    {
        private TickerPriceHistories()
        {
            Items = new List<TickerPriceHistory>();
        }

        public List<TickerPriceHistory> Items { get; private set; }

        public static TickerPriceHistories Create()
        {
            return new TickerPriceHistories();
        }

        public TickerPriceHistory GetOrCreate(string ticker)
        {
            var history = Items.FirstOrDefault(x => x.Ticker == ticker);
            if (history is not null) return history;

            history = TickerPriceHistory.Create(ticker);
            Items.Add(history);

            return history;
        }
    }
}
