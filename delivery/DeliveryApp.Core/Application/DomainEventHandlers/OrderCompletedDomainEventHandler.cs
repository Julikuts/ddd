using DeliveryApp.Core.Domain.OrderAggregate.DomainEvents;
using DeliveryApp.Core.Ports;
using MediatR;

namespace DeliveryApp.Core.Application.DomainEventHandlers;

public sealed class OrderCompletedDomainEventHandler : INotificationHandler<OrderCompletedDomainEvent>
{
    readonly IBusProducer _busProducer;
    public OrderCompletedDomainEventHandler(IBusProducer busProducer)
    {
        _busProducer = busProducer;
    }
    
    public async Task Handle(OrderCompletedDomainEvent notification, CancellationToken cancellationToken)
    {
        await _busProducer.PublishOrderCompletedDomainEvent(notification,cancellationToken);
    }
}