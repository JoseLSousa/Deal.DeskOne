using Deal.DeskOne.Domain.Aggregates.Request;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Deal.DeskOne.Infrastructure.Persistence.Configurations
{
    public sealed class RequestHistoryConfiguration : IEntityTypeConfiguration<RequestHistory>
    {
        public void Configure(EntityTypeBuilder<RequestHistory> builder)
        {
            builder.ToTable("RequestHistories");

            builder.HasKey(rh => rh.Id);

            builder.Property(h => h.FieldName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(h => h.OldValue)
                .HasMaxLength(2000);

            builder.Property(h => h.NewValue)
                .HasMaxLength(2000);

            builder.Property(h => h.Comment)
                .HasMaxLength(2000);

            builder.HasOne<RequestAggregate>()
                .WithMany(r => r.History)
                .HasForeignKey(h => h.RequestId)
                .IsRequired();
        }
    }
}
