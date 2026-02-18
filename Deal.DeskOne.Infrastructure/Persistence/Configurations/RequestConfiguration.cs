using Deal.DeskOne.Domain.Aggregates.Request;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Deal.DeskOne.Infrastructure.Persistence.Configurations
{
    public class RequestConfiguration : IEntityTypeConfiguration<RequestAggregate>
    {
        public void Configure(EntityTypeBuilder<RequestAggregate> builder)
        {
            builder.ToTable("Requests");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Version)
                .IsRowVersion();

            builder.Property(r => r.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(r => r.Description)
                .IsRequired()
                .HasMaxLength(2000);

            // Enums como integers (padrão do EF Core)
            builder.Property(r => r.Category)
                .IsRequired();

            builder.Property(r => r.Priority)
                .IsRequired();

            builder.Property(r => r.Status)
                .IsRequired();

            builder.Property(r => r.CreatedAt)
                .IsRequired();

            builder.Property(r => r.UpdatedAt);

            // Configura o relacionamento com o histórico
            builder.HasMany(r => r.History)
                .WithOne()
                .HasForeignKey("RequestId")
                .IsRequired();

            // Soft delete filter
            builder.HasQueryFilter(r => !r.IsDeleted);

        }
    }
}