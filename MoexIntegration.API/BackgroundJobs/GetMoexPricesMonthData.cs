using MediatR;
using MoexIntegration.Core.Application.Handling.Contracts;
using MoexIntegration.Core.Domain.Model.Prices;
using Quartz;

namespace MoexIntegration.API.BackgroundJobs
{
    [DisallowConcurrentExecution]
    public class GetMoexPricesMonthData : IJob
    {
        private readonly IMediator mediator;

        public GetMoexPricesMonthData(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await mediator.Send(new MoexGetPricesRequest { Period = PricePeriod.Month });
            await Task.CompletedTask;
        }
    }
}
