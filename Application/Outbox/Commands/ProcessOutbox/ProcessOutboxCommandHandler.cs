using Application.Common.Interfaces;
using Dapper;
using Domain.Outbox;
using MediatR;
using Newtonsoft.Json;

namespace Application.Outbox.Commands.ProcessOutbox
{
    internal class ProcessOutboxCommandHandler : IRequestHandler<ProcessOutboxCommand>
    {
        private readonly IMediator _mediator;

        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public ProcessOutboxCommandHandler(IMediator mediator, ISqlConnectionFactory sqlConnectionFactory)
        {
            _mediator = mediator;
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task Handle(ProcessOutboxCommand command, CancellationToken cancellationToken)
        {
            var connection = this._sqlConnectionFactory.GetOpenConnection();

            const string sql = "SELECT " +
                "[OutboxMessage].[Id], " +
                "[OutboxMessage].[Type], " +
                "[OutboxMessage].[Data] " +
                "FROM [app].[OutboxMessages] AS [OutboxMessage] " +
                "WHERE [OutboxMessage].[ProcessedDate] IS NULL";

            var messages = await connection.QueryAsync<OutboxMessage>(sql);
            var messagesList = messages.AsList();

            const string sqlUpdateProcessedDate = "UPDATE [app].[OutboxMessages] " +
                "SET [ProcessedDate] = @Date " +
                "WHERE [Id] = @Id";

            if (messagesList.Count > 0)
            {
                foreach (var message in messagesList)
                {
                    var request = JsonConvert.DeserializeObject(message.Data);

                    await this._mediator.Publish(request, cancellationToken);

                    await connection.ExecuteAsync(sqlUpdateProcessedDate, new
                    {
                        Date = DateTime.UtcNow,
                        message.Id
                    });
                }
            }
        }
    }
}
