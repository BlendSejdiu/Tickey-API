using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tickey.Data.Configurations;

public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.HasKey(t => t.Id);
        
        builder.Property(t => t.EventId).IsRequired();

        builder.Property(e => e.TicketType)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.Price)
            .IsRequired()
            .HasPrecision(10, 2);

        builder.Property(e => e.QuantityAvailable)
           .IsRequired();
    }
}
