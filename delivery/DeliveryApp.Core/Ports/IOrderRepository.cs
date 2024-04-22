using DeliveryApp.Core.Domain.OrderAggregate;
using Primitives;

namespace DeliveryApp.Core.Ports
{
    public interface IOrderRepository : IRepository<Order>
    {
        Order Add(Order order); // Добавить заказ
        void Update(Order order); // Обновить заказ
        Task<Order> GetAsync(Guid orderId);
        IEnumerable<Order> GetAllNotAssigned(); //Получить все неназначенные заказы
        IEnumerable<Order> GetAllAssigned(); //Получить все назначенные заказы

        Task<Order> GetByCourierId(Guid courierId);
    }
}