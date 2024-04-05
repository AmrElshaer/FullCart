using Domain.Common;
using Domain.Payments.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Payments
{
    public class Payment:Entity
    {
        public Guid OrderId { get; private set; }
        public PaymentStatus PaymentStatus { get; private set; }
        private Payment()
        {
            
        }
        public Payment(Guid orderId)
        {
            OrderId = orderId;
            PaymentStatus = PaymentStatus.ToPay;
            AddDomainEvent(new PaymentCreatedEvent() { OrderId = orderId, PaymentId=Id});
        }
    }
}
