using MediatR;
using MoexIntegration.Core.Application.Handling.Contracts;
using Quartz;

namespace MoexIntegration.API.BackgroundJobs
{
    public class GetAllMoexData : IJob
    {
        private readonly IMediator mediator;

        public GetAllMoexData(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await mediator.Send(new MoexGetAllSecuritiesRequest());
            await Task.CompletedTask;
        }
    }
}