using MediatR;

namespace MoexIntegration.Core.Application.Handling.Contracts
{
    public sealed record MoexGetDataRequest : IRequest<MoexGetDataResponse>
    {
        public required string Ticker { get; set; }
    }
}
