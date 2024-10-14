using Domain.Common;
using Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Orders.Events;

public class OrderPlacedEvent : DomainEvent
{
    public required OrderId OrderId { get; init; }

    public Guid CustomerId { get; init; }
}