using Domain.Comments;
using Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Comments.Persistence;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable("Comments");
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .IsRequired()
            .HasConversion(
                id => id.Value,
                value => CommentId.Create(value));
        builder.HasOne<Order>()
            .WithMany()
            .HasForeignKey(c => c.OrderId);
        builder.Property(x => x.Content)
            .HasMaxLength(500)
            .IsUnicode(false)
            .IsRequired();
    }
}