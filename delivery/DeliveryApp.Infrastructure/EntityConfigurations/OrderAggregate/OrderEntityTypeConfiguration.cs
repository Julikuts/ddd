using DeliveryApp.Core.Domain.OrderAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.EntityConfigurations.OrderAggregate
{
    class OrderEntityTypeConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> entityTypeBuilder)
        {
            entityTypeBuilder.ToTable("orders");

            entityTypeBuilder.HasKey(entity => entity.Id);

            entityTypeBuilder
                .Property(entity => entity.Id)
                .ValueGeneratedNever()
                .HasColumnName("id")
                .IsRequired();

            entityTypeBuilder
                .Property(entity => entity.CourierId)
                .HasColumnName("courier_id")
                .IsRequired(false);

            entityTypeBuilder
                .OwnsOne(entity => entity.Location, l =>
                {
                    l.Property(x => x.X).HasColumnName("location_x").IsRequired();
                    l.Property(y => y.Y).HasColumnName("location_y").IsRequired();
                    l.WithOwner();
                });

            entityTypeBuilder
                .OwnsOne(entity => entity.Weight, w =>
                {
                    w.Property(c => c.Value).HasColumnName("weight").IsRequired();
                    w.WithOwner();
                });

            entityTypeBuilder
                .OwnsOne(entity => entity.Status, a =>
                {
                    a.Property(c => c.Value).HasColumnName("status").IsRequired();
                    a.WithOwner();
                });
        }
    }
}