using Microsoft.EntityFrameworkCore.Metadata;

namespace Payphone.Infrastructure.EF.Configurations.Core;

public abstract class EfCoreConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : BaseModel
{
    public void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.CreatedAt).ValueGeneratedNever()
            .Metadata
            .SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

        builder.Property(x => x.UpdatedAt).ValueGeneratedNever();

        builder.Property(x => x.CreatedBy)
            .IsRequired()
            .Metadata
            .SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

        builder.Property(x => x.UpdatedAt)
            .IsRequired(false);

        builder.HasQueryFilter(x => !x.IsDeleted);
        ConfigureEf(builder);
    }

    protected abstract void ConfigureEf(EntityTypeBuilder<TEntity> builder);
}