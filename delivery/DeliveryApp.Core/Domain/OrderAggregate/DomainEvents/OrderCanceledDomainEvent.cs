using Primitives;

namespace DeliveryApp.Core.Domain.OrderAggregate.DomainEvents
{
    public sealed class OrderCanceledDomainEvent : IDomainEvent
    {
        public Guid Id { get; }
        public string Name { get; }
        public Order Order { get; }

        public OrderCanceledDomainEvent(Order order)
        {
            Id = Guid.NewGuid();
            Name = GetType().Name;
            Order = order;
        }
    }
}