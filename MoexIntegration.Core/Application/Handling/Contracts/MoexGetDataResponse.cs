
using MoexIntegration.Core.Domain.Model;

namespace MoexIntegration.Core.Application.Handling.Contracts
{
    public sealed record MoexGetDataResponse
    {
        public required StockInfo Stock { get; set; }
    }
}
