// Бизнес-правила Order:
using DeliveryApp.Core.Domain.CourierAggregate;
using DeliveryApp.Core.Domain.SharedKernel;
using Primitives;

namespace DeliveryApp.Core.Domain.OrderAggregate
{
    // Order - это заказ, он состоит из:

    public class Order : Aggregate
    {

        // CourierId (идентификатор исполнителя(курьера))
        public Guid? CourierId { get; protected set; }
        // Location (местоположение, куда нужно доставить заказ)
        public Location Location { get; protected set; }
        // Weight (вес заказа)
        public Weight Weight { get; protected set; }

        // Status (статус заказа)
        public OrderStatusEnum OrderStatus { get; protected set; }

        public Order(Guid id, Location location, Weight weight)
        {
            Id = id;
            Location = location;
            Weight = weight;
            OrderStatus = OrderStatusEnum.Created;
        }
        // Заказ может быть создан
        // При создании нужно передать Id, Location, 
        //Weight (Id передается на вход, так как основой для заказа будет являться Id корзины.
        // Поэтому мы не генерируем Id, а получаем на вход)
        // При создании заказа проставляется статус Created (создан)

        public Order Create(Guid id, Location location, Weight weight)
        {

            if (location is null)
            {
                throw new ArgumentNullException(nameof(location));
            }

            if (weight is null)
            {
                throw new ArgumentNullException(nameof(weight));
            }
            var order = new Order(id, location, weight);
            order.OrderStatus = OrderStatusEnum.Created;
            return order;
        }
        // Заказ может быть назначен на курьера
        // При назначении заказ переходит в статус Assigned (назначен на курьера),
        // а в поле CourierId прописывается Id курьера
        // Если заказ назначен на курьера, то курьер переходит в статус Busy

        public Courier Assign(Courier courier)
        {
            if (courier.CourierStatus == CourierStatusEnum.Busy ||
            courier.CourierStatus == CourierStatusEnum.NotAvailable)
                throw new Exception("Курьер не доступен");
            courier.CourierStart();
            CourierId = courier.Id;
            OrderStatus = OrderStatusEnum.Assigned;
            return courier;
        }
        // Заказ может быть завершен
        // При завершении заказ переходит в статус Completed (завершен)
        // А исполнитель становиться свободен (Ready)
        // Завершить можно только назначенный ранее заказ
        public void Complete()
        {
            if (OrderStatus != OrderStatusEnum.Assigned)
                throw new Exception("Невозможно завершить, заказ не взят в работу.");
            OrderStatus = OrderStatusEnum.Completed;
        }

    }
}

