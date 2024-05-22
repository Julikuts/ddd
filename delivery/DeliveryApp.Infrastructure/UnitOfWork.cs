using MediatR;
using Primitives;

namespace DeliveryApp.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMediator _mediator;
    public UnitOfWork(ApplicationDbContext dbContext, IMediator mediator)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        await DispatchDomainEventsAsynch();
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
    public async Task DispatchDomainEventsAsynch()
    {
        // Получаем доменные сущности типа Aggregate, в которых есть Domain Events
        var domainEntities = _dbContext.ChangeTracker
            .Entries<Aggregate>()
            .Where(x => x.Entity.GetDomainEvents().Any());

        // Копируем Domain Events в отдельную переменную
        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.GetDomainEvents())
            .ToList();

        // Очищаем Domain Events в доменной сущности
        domainEntities.ToList()
            .ForEach(entity => entity.Entity.ClearDomainEvents());

        // Отправляем все Domain Events в MediatR. Mediatr же их доставит в Handlers, где они и будут обработаны
        foreach (var domainEvent in domainEvents)
            await _mediator.Publish(domainEvent);
    }

}