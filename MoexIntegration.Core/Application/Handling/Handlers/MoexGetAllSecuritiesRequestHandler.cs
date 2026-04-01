using MediatR;
using MoexIntegration.Core.Abstractions;
using MoexIntegration.Core.Application.Handling.Contracts;
using MoexIntegration.Core.Domain.Model.Prices;
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

            //Создание групп активов
            var createGroupsResult = securities.CreateGroups();

            //обновление в кеше
            foreach (var group in securities.SecurityGroups)
            {
                await cacheService.WriteGroupSecurity(group.GroupeName, group.SecurityList);
            }
        }
    }
}

