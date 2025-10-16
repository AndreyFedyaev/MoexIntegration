using MediatR;

namespace MoexIntegration.Core.Application.Handling.Contracts
{
    public sealed record GetSecuritiesRequest : IRequest<GetSecuritiesResponse>;
}
