using DeliveryApp.Core.Domain.CourierAggregate;
using Primitives;

namespace DeliveryApp.Core.Ports
{
    public interface ICourierRepository : IRepository<Courier>
    {
        public Courier Add(Courier courier); // Добавить курьера
        public void Update(Courier courier); // Обновить курьера
        public Task<Courier> GetAsync(Guid courierId);
        public IEnumerable<Courier> GetAllReady(); // Получить всех свободных курьеров
        public IEnumerable<Courier> GetAllBusy(); // Получить всех занятых курьеров
    }
}