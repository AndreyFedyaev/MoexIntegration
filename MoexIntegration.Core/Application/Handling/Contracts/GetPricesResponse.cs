using MoexIntegration.Core.Domain.Model.Prices;

namespace MoexIntegration.Core.Application.Handling.Contracts
{
    public sealed record GetPricesResponse
    {
        public required List<TickerPriceHistories> PricesList { get; set; }
    }
}
