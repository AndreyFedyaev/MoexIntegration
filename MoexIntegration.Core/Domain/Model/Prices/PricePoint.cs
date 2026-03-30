namespace MoexIntegration.Core.Domain.Model.Prices
{
    /// <summary>
    /// Точка цены для линейного графика
    /// </summary>
    public class PricePoint
    {
        public DateTime? Time { get; private set; }

        public decimal Close { get; private set; }

        private PricePoint()
        {
        }

        private PricePoint(DateTime time, decimal close) : this()
        {
            Time = time;
            Close = close;
        }

        public static PricePoint Create(DateTime time, decimal close)
        {
            if (close <= 0) throw new ArgumentOutOfRangeException(nameof(close), "цена должна быть больше 0");

            return new PricePoint(time, close);
        }
    }
}
