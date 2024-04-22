using System.Data;
using DeliveryApp.Core.Domain.CourierAggregate;
using DeliveryApp.Core.Domain.OrderAggregate;
using DeliveryApp.Infrastructure.EntityConfigurations.CourierAggregate;
using DeliveryApp.Infrastructure.EntityConfigurations.OrderAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Primitives;

namespace DeliveryApp.Infrastructure
{
    public class ApplicationDbContext : DbContext, IUnitOfWork
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<Courier> Couriers { get; set; }
        
        private IDbContextTransaction _currentTransaction;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        
        public IDbContextTransaction GetCurrentTransaction() => _currentTransaction;

        public bool HasActiveTransaction => _currentTransaction != null;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
            // Order Aggregate
            modelBuilder.ApplyConfiguration(new OrderEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OrderStatusEntityTypeConfiguration());
            
            // Order statuses
            modelBuilder.Entity<OrderStatus>(b =>
            {
                var allStatuses = OrderStatus.List();
                b.HasData(allStatuses.Select(c=>new { c.Id,c.Name }));
            });
            
            
            // Courier Aggregate
            modelBuilder.ApplyConfiguration(new CourierEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CourierStatusEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new TransportEntityTypeConfiguration());
        
            // Courier statuses
            modelBuilder.Entity<CourierStatus>(b =>
            {
                var allStatuses = CourierStatus.List();
                b.HasData(allStatuses.Select(c=>new { c.Id,c.Name }));
            });
            
            // Courier transports
            modelBuilder.Entity<Transport>(b =>
            {
                var allTransports = Transport.GetAllPossibleTransports();
                b.HasData(allTransports.Select(c=>new { c.Id,c.Name }));
            });
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            await base.SaveChangesAsync(cancellationToken);
            return true;
        }
        
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            if (_currentTransaction != null) return null;

            _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            return _currentTransaction;
        }

        public async Task CommitTransactionAsync(IDbContextTransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (transaction != _currentTransaction) throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

            try
            {
                await SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public void RollbackTransaction()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }
    }
}