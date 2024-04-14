// Бизнес-правила OrderStatus:

// OrderStatus - это статус заказа, он состоит только из Value
// OrderStatus содержит 3 значения:
// Created (создан)
// Assigned (назначен на курьера)
// Completed (завершен)

namespace DeliveryApp.Core.Domain.OrderAggregate
{
    public enum OrderStatusEnum
    {
        /// <summary>
        /// Создан
        /// </summary>
        Created = 1,
        /// <summary>
        /// назначен на курьера
        /// </summary>
        Assigned = 2,
        /// <summary>
        /// Завершен
        /// </summary>
        Completed = 3
    }
}