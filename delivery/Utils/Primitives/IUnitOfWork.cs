using Microsoft.EntityFrameworkCore.Storage;

namespace Primitives;

public interface IUnitOfWork : IDisposable
{
    public Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default);

    public Task<IDbContextTransaction> BeginTransactionAsync();

    public Task CommitTransactionAsync(IDbContextTransaction transaction);
}