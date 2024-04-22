using DeliveryApp.Core.Domain.CourierAggregate;
using DeliveryApp.Core.Ports;
using Microsoft.EntityFrameworkCore;
using Primitives;

namespace DeliveryApp.Infrastructure.Adapters.Postgres
{
    public class CourierRepository : ICourierRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public IUnitOfWork UnitOfWork => throw new NotImplementedException();


        public CourierRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public Courier Add(Courier courier)
        {
            _dbContext.Attach(courier.Transport);
            return _dbContext.Couriers.Add(courier).Entity;
        }

        public void Update(Courier courier)
        {
            _dbContext.Attach(courier.Transport);
            _dbContext.Entry(courier).State = EntityState.Modified;
        }

        public Courier Get(Guid courierId)
        {
            var courier = _dbContext
                .Couriers
                .Include(x => x.Transport)
                .FirstOrDefault(o => o.Id == courierId);
            return courier;
        }

        public IEnumerable<Courier> GetAllReady()
        {
            var couriers = _dbContext
                .Couriers
                .Include(x => x.Transport)
                .Where(o => o.CourierStatus == CourierStatusEnum.Ready);
            return couriers;
        }

        public Task<Courier> GetAsync(Guid courierId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Courier> GetAllBusy()
        {
            throw new NotImplementedException();
        }

    }
}