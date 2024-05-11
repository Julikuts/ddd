using Primitives;

namespace DeliveryApp.Core.Domain.OrderAggregate.DomainEvents
{
    public sealed class OrderCreatedDomainEvent : IDomainEvent
    {
        public Guid Id { get; }
        public string Name { get; }
        public Order Order { get; }

        public OrderCreatedDomainEvent(Order order)
        {
            Id = Guid.NewGuid();
            Name = GetType().Name;
            Order = order;
        }
    }
}