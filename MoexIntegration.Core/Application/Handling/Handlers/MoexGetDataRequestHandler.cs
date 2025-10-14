using MoexIntegration.Core.Application.Handling.Contracts;
using MediatR;
using MoexIntegration.Core.Abstractions;
using MoexIntegration.Core.Domain.Model;

namespace MoexIntegration.Core.Application.Handling.Handlers
{
    public class MoexGetDataRequestHandler(IMoexApiRequest moexApiRequest)
        : IRequestHandler<MoexGetDataRequest, MoexGetDataResponse>
    {
        public async Task<MoexGetDataResponse> Handle(MoexGetDataRequest request, CancellationToken cancellationToken)
        {
            //создание объекта 
            var stock = StockInfo.Create(request.Ticker);
            ArgumentNullException.ThrowIfNull(stock);

            //получение названия
            var nameRequest = await moexApiRequest.GetName(request.Ticker);
            stock.AddName(nameRequest);

            //получение цены
            var priceRequest = await moexApiRequest.GetPrice(request.Ticker);
            stock.AddLastPrice(priceRequest);

            //получение количества акций
            var sharesOutstandingRequest = await moexApiRequest.GetSharesOutstanding(request.Ticker);
            stock.AddSharesOutstanding(sharesOutstandingRequest);

            //получение номера ISIN
            var isinRequest = await moexApiRequest.GetIsin(request.Ticker);
            stock.AddIsin(isinRequest);

            //рассчет капитализации
            stock.DefineMarketCapitalization();

            return new MoexGetDataResponse
            {
                Stock = stock
            };
        }
    }
}
