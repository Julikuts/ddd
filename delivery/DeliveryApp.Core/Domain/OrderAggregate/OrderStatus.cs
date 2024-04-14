// Бизнес-правила OrderStatus:

// OrderStatus - это статус заказа, он состоит только из Value
// OrderStatus содержит 3 значения:
// Created (создан)
// Assigned (назначен на курьера)
// Completed (завершен)

using CSharpFunctionalExtensions;

namespace DeliveryApp.Core.Domain.OrderAggregate
{
    public class OrderStatus:ValueObject
    {
        public OrderStatusEnum OrderStatusValue { get; protected set; }

        public OrderStatus(OrderStatusEnum status)
        {
            OrderStatusValue = status;
        }

        protected override IEnumerable<IComparable> GetEqualityComponents()
        {
           yield return OrderStatusValue;
        }
    }

}