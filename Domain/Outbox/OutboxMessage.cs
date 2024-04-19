using Domain.Common;

namespace Domain.Outbox
{
    public class OutboxMessage : Entity
    {
        public DateTime OccurredOn { get; private set; }

        public string Data { get; private set; } = default!;

        public DateTime? ProcessedDate { get; private set; }

        private OutboxMessage() { }

        public OutboxMessage(DateTime occurredOn, string data)
        {
            this.Id = Guid.NewGuid();
            this.OccurredOn = occurredOn;
            this.Data = data;
        }
    }
}
