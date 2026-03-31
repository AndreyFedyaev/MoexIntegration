using MediatR;
using MoexIntegration.Core.Abstractions;
using MoexIntegration.Core.Application.Handling.Contracts;
using MoexIntegration.Core.Domain.Model.Prices;
using MoexIntegration.Core.Domain.Model.Securities;
using System.Text.Json;

namespace MoexIntegration.Core.Application.Handling.Handlers
{
    public class MoexGetPricesRequestHandler(IMoexApiRequest moexApiRequest, ICacheService cacheService)
       : IRequestHandler<MoexGetPricesRequest>
    {
        public async Task Handle(MoexGetPricesRequest request, CancellationToken cancellationToken)
        {
            string cacheKey = request.Period switch
            {
                PricePeriod.Day => "PriceDay",
                PricePeriod.Week => "PriceWeek",
                PricePeriod.Month => "PriceMonth",
                PricePeriod.Year => "PriceYear",
                _ => throw new ArgumentOutOfRangeException(nameof(request.Period), request.Period, null)
            };

            //Получение списка групп
            var groups = await cacheService.GetGroups();

            //Создаем объект списка групп и заполняем данными из кеша
            List<SecurityGroup> allGrupsData = [];
            foreach (var group in groups)
            {
                var securities = await cacheService.GetGroupSecurities(group);

                allGrupsData.Add(new SecurityGroup { 
                    GroupeName = group,
                    SecurityList = securities
                });
            }

            foreach (var groupData in allGrupsData)
            {
                var groupPrices = TickerPriceHistories.Create();

                //Получение цен для всех активов группы
                foreach (var ticker in groupData.SecurityList)
                {
                    var tickerData = groupPrices.GetOrCreate(ticker.Ticker);
                    JsonElement tickerPriceHistory;

                    switch (request.Period)
                    {
                        case PricePeriod.Day:
                            tickerPriceHistory = await moexApiRequest.GetCandleHistoryDay(ticker.Ticker, 10);
                            break;
                        case PricePeriod.Week:
                            throw new NotImplementedException("пока не реализовано.");
                        //tickerPriceHistory = await moexApiRequest.GetCandleHistoryWeek(ticker.Ticker, 10);
                        //break;
                        case PricePeriod.Month:
                            throw new NotImplementedException("пока не реализовано.");
                        //tickerPriceHistory = await moexApiRequest.GetCandleHistoryMonth(ticker.Ticker, 10);
                        //break;
                        case PricePeriod.Year:
                            throw new NotImplementedException("пока не реализовано.");
                        //tickerPriceHistory = await moexApiRequest.GetCandleHistoryYear(ticker.Ticker, 10);
                        //break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(request.Period), request.Period, null);
                    }

                    tickerData.AddPrice(tickerPriceHistory);
                }

                //Записываем цены всех активов текущей группы в кеш
                await cacheService.WriteGroupPrices(groupData.GroupeName, cacheKey, groupPrices);
                
            }
        }
    }
}
