using DeliveryApp.Core.Domain.OrderAggregate.DomainEvents;
using DeliveryApp.Core.Ports;
using MediatR;

namespace DeliveryApp.Core.Application.DomainEventHandlers;

public sealed class OrderAssignedDomainEventHandler : INotificationHandler<OrderAssignedDomainEvent>
{
    readonly IBusProducer _busProducer;
    public OrderAssignedDomainEventHandler(IBusProducer busProducer)
    {
        _busProducer = busProducer;
    }
    
    public async Task Handle(OrderAssignedDomainEvent notification, CancellationToken cancellationToken)
    {
        await _busProducer.PublishOrderAssignedDomainEvent(notification,cancellationToken);
    }
}