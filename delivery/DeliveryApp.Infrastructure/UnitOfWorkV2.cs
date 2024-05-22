using DeliveryApp.Infrastructure.Entities;
using DeliveryApp.Infrastructure;
using Newtonsoft.Json;
using Primitives;

namespace DeliveryApp.Infrastructure;

public class UnitOfWorkV2 : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;

    public UnitOfWorkV2(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }
    
    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        await SaveDomainEventsInOutboxEventsAsync();
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
        
    async Task SaveDomainEventsInOutboxEventsAsync()
    {
        // Достаем доменные события из Aggregate и преобразовываем их к OutboxMessage
        var outboxMessages = _dbContext.ChangeTracker
            .Entries<Aggregate>()
            .Select(x => x.Entity)
            .SelectMany(aggregate =>
                {
                    var domainEvents = aggregate.GetDomainEvents();
                    aggregate.ClearDomainEvents();
                    return domainEvents;
                }
            )
            .Select(domainEvent => new OutboxMessage()
            {
                Id = domainEvent.EventId,
                OccuredOnUtc = DateTime.UtcNow,
                Type = domainEvent.GetType().Name,
                Content = JsonConvert.SerializeObject(
                    domainEvent,
                    new JsonSerializerSettings()
                    {
                        TypeNameHandling = TypeNameHandling.All
                    })  
            })
            .ToList();
        
        // Добавяляем OutboxMessage в dbContext, после выхода из метода они и сам Aggregate будут сохранены
        await _dbContext.Set<OutboxMessage>().AddRangeAsync(outboxMessages);
    }
}