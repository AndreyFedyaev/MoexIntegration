using MoexIntegration.Core.Domain.Model.Securities;

namespace MoexIntegration.Core.Application.Handling.Contracts
{
    public sealed record GetSecuritiesResponse
    {
        public required List<Security> SecuritiesList { get; set; }
    }
}
