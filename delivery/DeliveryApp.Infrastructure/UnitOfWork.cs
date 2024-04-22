using Microsoft.EntityFrameworkCore.Storage;
using Primitives;

namespace DeliveryApp.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;

    public UnitOfWork(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public Task<IDbContextTransaction> BeginTransactionAsync()
    {
        throw new NotImplementedException();
    }

    public Task CommitTransactionAsync(IDbContextTransaction transaction)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }


    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
    }
}