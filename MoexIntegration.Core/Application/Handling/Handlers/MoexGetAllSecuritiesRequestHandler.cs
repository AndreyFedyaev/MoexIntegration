using MediatR;
using MoexIntegration.Core.Abstractions;
using MoexIntegration.Core.Application.Handling.Contracts;
using MoexIntegration.Core.Domain.Model.Securities;

namespace MoexIntegration.Core.Application.Handling.Handlers
{
    public class MoexGetAllSecuritiesRequestHandler(IMoexApiRequest moexApiRequest, ICacheService cacheService)
        : IRequestHandler<MoexGetAllSecuritiesRequest>
    {
        public async Task Handle(MoexGetAllSecuritiesRequest request, CancellationToken cancellationToken)
        {
            //создание объекта 
            var securities = Securities.Create();
            ArgumentNullException.ThrowIfNull(securities);

            //получение списка всех акций, торгуемых на MOEX
            var securitiesRequest = await moexApiRequest.GetAllSecurities();
            securities.CreateSecurities(securitiesRequest);

            //обновление в кеше
            var casheUpdateResult = cacheService.WriteSecurities(securities.SecurityList);
        }
    }
}
