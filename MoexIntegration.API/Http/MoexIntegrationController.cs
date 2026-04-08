using MediatR;
using Microsoft.AspNetCore.Mvc;
using MoexIntegration.Core.Application.Handling.Contracts;
using MoexIntegration.Core.Domain.Model.Prices;

namespace MoexIntegration.API.Http
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoexIntegrationController(IMediator mediator) : ControllerBase
    {
        // Получение списка всех активов
        [HttpGet("getsecurities")]
        public async Task<GetSecuritiesResponse> GetSecuritiesList()
        {
            var result = await mediator.Send(new GetSecuritiesRequest());
            return result;
        }

        // Получение цен всех активов за выбранный период
        [HttpGet("prices/{period}")]
        public async Task<ActionResult<GetPricesResponse>> GetAllPrices(string period)
        {
            if (!Enum.TryParse<PricePeriod>(period, true, out var pricePeriod))
            {
                return BadRequest("Некорректный период. Доступные значения: day, week, month, year");
            }

            var result = await mediator.Send(new GetPricesRequest { Period = pricePeriod });

            return Ok(result);
        }
    }
}
