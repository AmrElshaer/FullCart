using Application.Outbox.Commands.ProcessOutbox;
using MediatR;
using Quartz;

namespace Infrastructure.Outbox
{
    [DisallowConcurrentExecution]
    public class ProcessOutboxJob : IJob
    {
        private readonly IMediator _mediator;

        public ProcessOutboxJob(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            await _mediator.Send(new ProcessOutboxCommand());
        }
    }
}
