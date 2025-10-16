using Microsoft.AspNetCore.Mvc;
using MediatR;
using MoexIntegration.Core.Application.Handling.Contracts;

namespace MoexIntegration.API.Http
{
    [ApiController]
    public class Controller(IMediator mediator) : ControllerBase
    {
        //получение данных по тикеру
        [HttpGet("getstockdata/{TICKER}")]
        public async Task<MoexGetDataResponse> GetTickerData(string ticker)
        {
            var result = await mediator.Send(new MoexGetDataRequest { Ticker = ticker});
            return result;
        }

        //выгрузка из кеша списка всех акций
        [HttpGet("getsecurities")]
        public async Task<GetSecuritiesResponse> GetSecuritiesList()
        {
            var result = await mediator.Send(new GetSecuritiesRequest());    
            return result;
        }

    }
}
