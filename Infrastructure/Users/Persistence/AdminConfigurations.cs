using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Users.Persistence;

public class AdminConfigurations: IEntityTypeConfiguration<Admin>
{
    public void Configure(EntityTypeBuilder<Admin> builder)
    {
        builder.ToTable("Admins");
        builder.HasOne(c => c.User)
            .WithOne(u => u.Admin)
            .HasForeignKey<Admin>(c => c.Id)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
