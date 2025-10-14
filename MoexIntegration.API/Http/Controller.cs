using Microsoft.AspNetCore.Mvc;
using MediatR;
using MoexIntegration.Core.Application.Handling.Contracts;

namespace MoexIntegration.API.Http
{
    [ApiController]
    public class Controller(IMediator mediator) : ControllerBase
    {
        [HttpGet("getstockdata/{TICKER}")]
        public async Task<MoexGetDataResponse> GetTickerData(string ticker)
        {
            //await mediator.Send(new MoexGetAllSecuritiesRequest());       //временно для тестов

            var result = await mediator.Send(new MoexGetDataRequest { Ticker = ticker});
            return result;
        }

    }
}
