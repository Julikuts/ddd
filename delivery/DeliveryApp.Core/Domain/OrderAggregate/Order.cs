using System.Diagnostics.CodeAnalysis;
using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.CourierAggregate;
using DeliveryApp.Core.Domain.OrderAggregate.DomainEvents;
using DeliveryApp.Core.Domain.SharedKernel;
using Primitives;

namespace DeliveryApp.Core.Domain.OrderAggregate
{
    /// <summary>
    ///     Заказ
    /// </summary>
    public class Order : Aggregate
    {
        public static class Errors
        {
            public static Error CantCompletedNotAssignedOrder()
            {
                return new($"{nameof(Order).ToLowerInvariant()}.cant.completed.not.sssigned.order",
                    "Нельза завершить заказ, который не был назначен");
            }
            
            public static Error CantAssignOrderToBusyCourier(Guid courierId)
            {
                return new($"{nameof(Order).ToLowerInvariant()}.cant.assign.order.to.busy.courier",
                    $"Нельза назначить заказ на курьера, который занят. Id курьера = {courierId}");
            }
        }

        /// <summary>
        /// Идентификатор исполнителя (курьера)
        /// </summary>
        public Guid? CourierId { get; private set; }

        /// <summary>
        /// Местоположение, куда нужно доставить заказ
        /// </summary>
        public Location Location { get; private set; }

        /// <summary>
        /// Вес
        /// </summary>
        public Weight Weight { get; private set; }

        /// <summary>
        /// Статус
        /// </summary>
        public Status Status { get; private set; }

        /// <summary>
        /// Ctr
        /// </summary>
        [ExcludeFromCodeCoverage]
        private Order()
        { }

        /// <summary>
        ///     Ctr
        /// </summary>
        /// <param name="orderId">Идентификатор заказа</param>
        /// <param name="location">Геопозиция</param>
        /// <param name="weight">Вес</param>
        private Order(Guid orderId, Location location, Weight weight)
        {
            Id = orderId;
            Location = location;
            Weight = weight;
            Status = Status.Created;
            
            // отправляем сообщение
            RaiseDomainEvent(new OrderCreatedDomainEvent(this));
        }

        /// <summary>
        /// Factory Method
        /// </summary>
        /// <param name="orderId">Идентификатор заказа</param>
        /// <param name="location">Геопозиция</param>
        /// <param name="weight">Вес</param>
        /// <returns>Результат</returns>
        public static Result<Order, Error> Create(Guid orderId, Location location, Weight weight)
        {
            if (orderId == Guid.Empty) return GeneralErrors.ValueIsRequired(nameof(orderId));
            if (location == null) return GeneralErrors.ValueIsRequired(nameof(location));
            if (weight == null) return GeneralErrors.ValueIsRequired(nameof(weight));
            
            return new Order(orderId, location, weight);
        }

        /// <summary>
        /// Назначить заказ на курьера
        /// </summary>
        /// <param name="courier">Курьер</param>
        /// <returns>Результат</returns>
        public Result<object, Error> AssignToCourier(Courier courier)
        {
            if (courier == null) return GeneralErrors.ValueIsRequired(nameof(courier));
            if (courier.Status == CourierAggregate.Status.Busy)
                return Errors.CantAssignOrderToBusyCourier(courier.Id);

            CourierId = courier.Id;
            Status = Status.Assigned;
            courier.InWork();
            

            // Сервис доставки должен отправлять нотификации через сервис Notification (см схему выше):
            // При назначении заказа на курьера
            RaiseDomainEvent(new OrderAssignedDomainEvent(this));


            return new object();
        }

        /// <summary>
        /// Завершить выполнение заказа
        /// </summary>
        /// <returns>Результат</returns>
        public Result<object, Error> Complete()
        {
            if (Status != Status.Assigned) return Errors.CantCompletedNotAssignedOrder();
            Status = Status.Completed;

            // Сервис доставки должен отправлять нотификации через сервис Notification (см схему выше):
            // При завершении заказа
            RaiseDomainEvent(new OrderCompletedDomainEvent(this));

            return new object();
        }
    }
}