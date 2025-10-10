using Microsoft.AspNetCore.Mvc;
using MediatR;
using MoexIntegration.Core.Application.Handling.Contracts;

namespace MoexIntegration.API.Http
{
    [ApiController]
    public class Controller(IMediator mediator) : ControllerBase
    {
        [HttpGet("gettickerdata/{TICKER}")]
        public async Task<MoexGetDataResponse> GetTickerData(string ticker)
        {
            var result = await mediator.Send(new MoexGetDataRequest { Ticker = ticker});
            return result;
        }

    }
}
