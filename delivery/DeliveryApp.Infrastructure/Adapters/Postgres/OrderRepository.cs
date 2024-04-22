using DeliveryApp.Core.Domain.OrderAggregate;
using DeliveryApp.Core.Ports;
using Microsoft.EntityFrameworkCore;
using Primitives;

namespace DeliveryApp.Infrastructure.Adapters.Postgres
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public IUnitOfWork UnitOfWork => throw new NotImplementedException();


        public OrderRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public void Add(Order order)
        {
            _dbContext.Orders.Add(order);
        }

        public void Update(Order order)
        {
            _dbContext.Entry(order).State = EntityState.Modified;
        }

        public Order Get(Guid orderId)
        {
            var order = _dbContext
                .Orders
                .FirstOrDefault(o => o.Id == orderId);
            return order;
        }

        public IEnumerable<Order> GetAllNotAssigned()
        {
            var orders = _dbContext
                .Orders
                .Where(o => o.OrderStatus == OrderStatusEnum.Created);
            return orders;
        }

        public IEnumerable<Order> GetAllAssigned()
        {
            var orders = _dbContext
                .Orders
                .Where(o => o.OrderStatus == OrderStatusEnum.Assigned);
            return orders;
        }

        Order IOrderRepository.Add(Order order)
        {
            throw new NotImplementedException();
        }

        public Task<Order> GetAsync(Guid orderId)
        {
            throw new NotImplementedException();
        }

        public Task<Order> GetByCourierId(Guid courierId)
        {
            throw new NotImplementedException();
        }

    }
}