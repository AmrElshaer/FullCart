using Domain.Common;
using Domain.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Payments.Events
{
    public class PaymentCreatedEvent : DomainEvent
    {
    
        public Guid PaymentId { get; init; }

        public Guid OrderId { get; init; }
    }
}
