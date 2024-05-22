using DeliveryApp.Core.Domain.CourierAggregate;
using DeliveryApp.Core.Domain.OrderAggregate;
using DeliveryApp.Infrastructure.Entities;
using DeliveryApp.Infrastructure.EntityConfigurations.CourierAggregate;
using DeliveryApp.Infrastructure.EntityConfigurations.OrderAggregate;
using DeliveryApp.Infrastructure.EntityConfigurations.Outbox;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApp.Infrastructure;

public class ApplicationDbContext : DbContext
{
    public DbSet<Order> Orders { get; set; }
    public DbSet<Courier> Couriers { get; set; }

    public DbSet<OutboxMessage> OutboxMessages { get; set; }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Order Aggregate
        modelBuilder.ApplyConfiguration(new OrderEntityTypeConfiguration());

        // Courier Aggregate
        modelBuilder.ApplyConfiguration(new CourierEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new TransportEntityTypeConfiguration());

        // Courier transports
        modelBuilder.Entity<Transport>(b =>
        {
            var allTransports = Transport.List();
            b.HasData(allTransports.Select(c => new { c.Id, c.Name, c.Speed, c.Capacity.Value }));
        });
        modelBuilder.ApplyConfiguration(new OutboxEntityTypeConfiguration());
    }
}