using MediatR;
using Quartz;

namespace DeliveryApp.Api.Adapters.BackgroundJobs;

public class GetMoexPricesData : IJob
{
    private readonly IMediator mediator;

    public GetMoexPricesData(IMediator mediator)
    {
        this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task Execute(IJobExecutionContext context)
    {
        // TODO: вызвать новый handler после его добавления в проект.
        // await mediator.Send(new MoexGetPricesRequest());
        await Task.CompletedTask;
    }
}
