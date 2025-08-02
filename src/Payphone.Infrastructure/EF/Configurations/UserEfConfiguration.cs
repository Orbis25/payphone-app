using Payphone.Domain.Models;

namespace Payphone.Infrastructure.EF.Configurations;

public class UserEfConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(x => x.FullName).IsRequired().HasMaxLength(50);
    }
}