using DeliveryApp.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.EntityConfigurations.Outbox
{
    class OutboxEntityTypeConfiguration : IEntityTypeConfiguration<OutboxMessage>
    {
        public void Configure(EntityTypeBuilder<OutboxMessage> goodConfiguration)
        {
            goodConfiguration
                .ToTable("outbox");

            goodConfiguration
                .Property(entity => entity.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");

            goodConfiguration
                .Property(entity => entity.Type)
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("type")
                .IsRequired();

            goodConfiguration
                .Property(entity => entity.Content)
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("content")
                .IsRequired();

            goodConfiguration
                .Property(entity => entity.OccuredOnUtc)
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("occuredOnUtc")
                .IsRequired();

            goodConfiguration
                .Property(entity => entity.ProcessedOnUtc)
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("ProcessedOnUtc")
                .IsRequired(false);

            goodConfiguration
                .Property(entity => entity.Error)
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("error")
                .IsRequired(false);
        }
    }
}