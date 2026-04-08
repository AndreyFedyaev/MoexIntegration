using MediatR;
using MoexIntegration.Core.Domain.Model.Prices;

namespace MoexIntegration.Core.Application.Handling.Contracts
{
    public sealed record GetPricesRequest : IRequest<GetPricesResponse>
    {
        public required PricePeriod Period { get; set; }
    }
}
