using Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Common.Behaviours;

public class DispatchingIntegrationEventDecorator<TNotification> : INotificationHandler
   <TNotification> where TNotification : INotification
{
   private readonly ICartDbContext _dbContext;
   private readonly INotificationHandler<TNotification> _inner;

   public DispatchingIntegrationEventDecorator(ICartDbContext dbContext, INotificationHandler<TNotification> inner)
   {
      _dbContext = dbContext;
      _inner = inner;
   }
   public Task Handle(TNotification notification, CancellationToken cancellationToken)
   {
      _inner.Handle(notification, cancellationToken);
      return  _dbContext.DispatchDomainEvents();
      
   }
}
