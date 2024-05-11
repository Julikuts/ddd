using DeliveryApp.Core.Domain.OrderAggregate;
using Primitives;

namespace DeliveryApp.Core.Ports
{
    /// <summary>
    /// Repository для Aggregate Order
    /// </summary>
    public interface IOrderRepository : IRepository<Order>
    {
        /// <summary>
        /// Добавить
        /// </summary>
        /// <param name="order">Заказ</param>
        /// <returns>Заказ</returns>
        void Add(Order order);

        /// <summary>
        /// Обновить
        /// </summary>
        /// <param name="order">Заказ</param>
        void Update(Order order);

        /// <summary>
        /// Получить
        /// </summary>
        /// <param name="orderId">Идентификатор</param>
        /// <returns>Заказ</returns>
        Order Get(Guid orderId);

        /// <summary>
        /// Получить все нераспределенные заказы
        /// </summary>
        /// <returns>Заказы</returns>
        IEnumerable<Order> GetAllNotAssigned();

        /// <summary>
        /// Получить все распределенные заказы
        /// </summary>
        /// <returns>Заказы</returns>
        IEnumerable<Order> GetAllAssigned();
    }
}