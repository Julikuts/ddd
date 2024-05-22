using Primitives;

namespace DeliveryApp.Core.Domain.OrderAggregate.DomainEvents
{
    public sealed class OrderCanceledDomainEvent : IDomainEvent
    {
        public Guid Id { get; }
        public string Name { get; }
        public Order Order { get; }
        public Guid EventId { get ; set;}

        public OrderCanceledDomainEvent(Order order)
        {
            Id = Guid.NewGuid();
            EventId = Id;
            Name = GetType().Name;
            Order = order;
        }
    }
}