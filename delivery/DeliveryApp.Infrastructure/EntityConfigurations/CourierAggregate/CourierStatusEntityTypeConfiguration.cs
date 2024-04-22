using DeliveryApp.Core.Domain.CourierAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.EntityConfigurations.CourierAggregate
{
    class CourierStatusEntityTypeConfiguration : IEntityTypeConfiguration<CourierStatus>
    {
        public void Configure(EntityTypeBuilder<CourierStatus> entityTypeBuilder)
        {
            entityTypeBuilder.ToTable("courier_statuses");

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
        }
    }
}