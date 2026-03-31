using MediatR;
using MoexIntegration.Core.Application.Handling.Contracts;
using MoexIntegration.Core.Domain.Model.Prices;
using Quartz;

namespace MoexIntegration.API.BackgroundJobs
{
    public class GetMoexPricesData : IJob
    {
        private readonly IMediator mediator;

        public GetMoexPricesData(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await mediator.Send(new MoexGetPricesRequest{ Period = PricePeriod.Day });
            await Task.CompletedTask;
        }
    }
}
