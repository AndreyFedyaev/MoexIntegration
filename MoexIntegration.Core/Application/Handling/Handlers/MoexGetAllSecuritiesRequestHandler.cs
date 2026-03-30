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
                var casheUpdateResult = cacheService.WriteSecurityGroups($"MoexIntegration:Groups:{group.GroupeName}:SecuritiesList", group.SecurityList);
            }
            //var casheUpdateResult = cacheService.WriteSecurities(securities.SecurityList);


            //для тестов
            //var allPrices = TickerPriceHistories.Create();

            //foreach (var security in securities.SecurityList)
            //{
            //    Console.WriteLine($"Читаем данные для -  {security.Ticker}");

            //    var ticker = allPrices.GetOrCreate(security.Ticker);

            //    //вылетают иногда ошибки

            //    //долго выполняется, нужно что-то думать

            //    //иногда пустые результаты

            //    var tickerPriceHistory = await moexApiRequest.GetCandleHistoryDay(security.Ticker, 10);

            //    ticker.AddPrice(tickerPriceHistory);

            //    Console.WriteLine($"Добавлен тикер {ticker.Ticker} | allPrices.Count = {allPrices.Items.Count}");
            //}

           var testdata = await moexApiRequest.GetCandleHistoryDay(securities.SecurityList[0].Ticker, 10);
        }
    }
}
