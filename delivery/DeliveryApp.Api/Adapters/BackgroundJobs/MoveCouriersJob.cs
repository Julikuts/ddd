using MediatR;
using Quartz;

namespace DeliveryApp.Api.Adapters.BackgroundJobs
{
    [DisallowConcurrentExecution]
    public class MoveCouriersJob : IJob
    {
        private readonly IMediator _mediator;

        public MoveCouriersJob(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var moveToOrderCommand = new Core.Application.UseCases.Commands.MoveToOrder.Command();
            await _mediator.Send(moveToOrderCommand);
        }
    }
}