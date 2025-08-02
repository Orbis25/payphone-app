namespace Payphone.Infrastructure.EF.Configurations.Core;

public abstract class EfCoreConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
       where TEntity : BaseModel
{
    public void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.Property(x => x.CreatedAt).ValueGeneratedNever();
        builder.HasQueryFilter(x => !x.IsDeleted);
        ConfigureEf(builder);
    }

    protected abstract void ConfigureEf(EntityTypeBuilder<TEntity> builder);
}
