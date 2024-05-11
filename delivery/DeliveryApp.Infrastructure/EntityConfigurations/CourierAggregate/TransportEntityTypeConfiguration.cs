using DeliveryApp.Core.Domain.CourierAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.EntityConfigurations.CourierAggregate
{
    class TransportEntityTypeConfiguration : IEntityTypeConfiguration<Transport>
    {
        public void Configure(EntityTypeBuilder<Transport> entityTypeBuilder)
        {
            entityTypeBuilder.ToTable("transports");

            entityTypeBuilder.HasKey(entity => entity.Id);

            entityTypeBuilder
                .Property(entity => entity.Id)
                .ValueGeneratedNever()
                .HasColumnName("id")
                .IsRequired();

            entityTypeBuilder
                .Property(entity => entity.Name)
                .HasColumnName("name")
                .IsRequired();

            entityTypeBuilder
                .Property(entity => entity.Speed)
                .HasColumnName("speed")
                .IsRequired();

            entityTypeBuilder
                .OwnsOne(entity => entity.Capacity, c =>
                {
                    c.Property(v => v.Value).HasColumnName("capacity").IsRequired();
                    c.WithOwner();
                });
        }
    }
}