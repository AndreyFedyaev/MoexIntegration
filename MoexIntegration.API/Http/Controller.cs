using Microsoft.AspNetCore.Mvc;
using MediatR;
using MoexIntegration.Core.Application.Handling.Contracts;

namespace MoexIntegration.API.Http
{
    [ApiController]
    public class Controller(IMediator mediator) : ControllerBase
    {
        //получение списка всех активов
        [HttpGet("getsecurities")]
        public async Task<GetSecuritiesResponse> GetSecuritiesList()
        {
            var result = await mediator.Send(new GetSecuritiesRequest());    
            return result;
        }
    }
}
