using MoexIntegration.Core.Application.Handling.Contracts;
using MediatR;
using MoexIntegration.Core.Abstractions;
using MoexIntegration.Core.Domain.Model.Prices;
using MoexIntegration.Core.Domain.Model.Securities;
using System.Text.Json;

namespace MoexIntegration.Core.Application.Handling.Handlers
{
    public class GetPricesRequestHandler(ICacheService cacheService)
        : IRequestHandler<GetPricesRequest, GetPricesResponse>
    {
        public async Task<GetPricesResponse> Handle(GetPricesRequest request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"--- GetPricesRequestHandler: старт работы | Период: {request.Period}");

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

                allGrupsData.Add(new SecurityGroup
                {
                    GroupeName = group,
                    SecurityList = securities
                });
            }

            foreach (var groupData in allGrupsData)
            {
                //var groupPrices = TickerPriceHistories.Create();

                //var addNewPriceResult = false;

                //Получение цен для всех активов группы
                //foreach (var ticker in groupData.SecurityList)
                //{
                //    var tickerData = groupPrices.GetOrCreate(ticker.Ticker);

                //    try
                //    {
                //        JsonElement tickerPriceHistory;

                //        выгрузка свечей:
                //         1    — 1 минута
                //         10   — 10 минут
                //         60   — 1 час
                //         24   — 1 день
                //         7    — 1 неделя
                //         31   — 1 месяц
                //         4    — 1 квартал

                //        switch (request.Period)
                //        {
                //            case PricePeriod.Day:
                //                tickerPriceHistory = await moexApiRequest.GetCandleHistoryDay(ticker.Ticker, 10);   // 10 минут
                //                break;
                //            case PricePeriod.Week:
                //                tickerPriceHistory = await moexApiRequest.GetCandleHistoryWeek(ticker.Ticker, 60);  // 1 час
                //                break;
                //            case PricePeriod.Month:
                //                tickerPriceHistory = await moexApiRequest.GetCandleHistoryMonth(ticker.Ticker, 24); // 1 день
                //                break;
                //            case PricePeriod.Year:
                //                tickerPriceHistory = await moexApiRequest.GetCandleHistoryYear(ticker.Ticker, 7);  // 1 неделя
                //                break;
                //            default:
                //                throw new ArgumentOutOfRangeException(nameof(request.Period), request.Period, null);
                //        }

                //        addNewPriceResult = tickerData.AddPrice(tickerPriceHistory);
                //    }
                //    catch (HttpRequestException ex)
                //    {
                //        Console.WriteLine($"Ticker: {ticker.Ticker} | Ошибка HTTP: {ex.Message}");
                //        continue;
                //    }
                //    catch (TaskCanceledException ex)
                //    {
                //        Console.WriteLine($"Ticker: {ticker.Ticker} | Таймаут: {ex.Message}");
                //        continue;
                //    }
                //    catch (Exception ex)
                //    {
                //        Console.WriteLine($"Ticker: {ticker.Ticker} | Exception: {ex.Message}");
                //        continue;
                //    }
            }

            return new GetPricesResponse { PricesList = [] };
        }
    }
}
