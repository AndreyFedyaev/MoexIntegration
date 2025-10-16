using MoexIntegration.Core.Application.Handling.Contracts;
using MediatR;
using MoexIntegration.Core.Abstractions;

namespace MoexIntegration.Core.Application.Handling.Handlers
{
    public class GetSecuritiesRequestHandler(ICacheService cacheService)
        : IRequestHandler<GetSecuritiesRequest, GetSecuritiesResponse>
    {
        public async Task<GetSecuritiesResponse> Handle(GetSecuritiesRequest request, CancellationToken cancellationToken)
        {
            //получаем данные из кеша
            var result = await cacheService.GetSecurities();
            ArgumentNullException.ThrowIfNull(result);

            return new GetSecuritiesResponse{ SecuritiesList = result }; 
        }
    }
}
