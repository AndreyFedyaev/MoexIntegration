using MediatR;
using MoexIntegration.Core.Domain.Model.Prices;

namespace MoexIntegration.Core.Application.Handling.Contracts
{
    public class MoexGetPricesRequest : IRequest
    {
        public required PricePeriod Period { get; set; }
    }
}
