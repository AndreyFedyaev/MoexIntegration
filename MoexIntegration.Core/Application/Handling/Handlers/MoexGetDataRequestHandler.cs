using MoexIntegration.Core.Application.Handling.Contracts;
using MediatR;
using MoexIntegration.Core.Abstractions;
using System.Globalization;

namespace MoexIntegration.Core.Application.Handling.Handlers
{
    public class MoexGetDataRequestHandler(IMoexApiRequest moexApiRequest)
        : IRequestHandler<MoexGetDataRequest, MoexGetDataResponse>
    {
        public async Task<MoexGetDataResponse> Handle(MoexGetDataRequest request, CancellationToken cancellationToken)
        {
            var marketdata = await moexApiRequest.GetPrice(request.Ticker);

            var colsPrice = marketdata.GetProperty("columns").EnumerateArray().Select(x => x.GetString()).ToList();
            var rowPrice = marketdata.GetProperty("data")[0].EnumerateArray().Select(x => x.ToString()).ToList();
            double price = double.Parse(rowPrice[colsPrice.IndexOf("LAST")], CultureInfo.InvariantCulture);

            return new MoexGetDataResponse
            {
                PRICE = price,
                ISSUESIZE = 5555555
            };
        }
    }
}
