using MoexIntegration.Core.Application.Handling.Contracts;
using MediatR;
using MoexIntegration.Core.Abstractions;
using MoexIntegration.Core.Domain.Model.Securities;

namespace MoexIntegration.Core.Application.Handling.Handlers
{
    public class GetSecuritiesRequestHandler(ICacheService cacheService)
        : IRequestHandler<GetSecuritiesRequest, GetSecuritiesResponse>
    {
        public async Task<GetSecuritiesResponse> Handle(GetSecuritiesRequest request, CancellationToken cancellationToken)
        {
            //TODO Не актуальный хендлер


            ////получаем данные из кеша
            //var result = await cacheService.GetSecurities();
            //ArgumentNullException.ThrowIfNull(result);

            //return new GetSecuritiesResponse{ SecuritiesList = result }; 

            return new GetSecuritiesResponse { SecuritiesList = [] };
        }
    }
}
