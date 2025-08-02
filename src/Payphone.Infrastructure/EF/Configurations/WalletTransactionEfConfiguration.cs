namespace Payphone.Infrastructure.EF.Configurations;

public class WalletTransactionEfConfiguration : EfCoreConfiguration<WalletTransaction>
{
    protected override void ConfigureEf(EntityTypeBuilder<WalletTransaction> builder)
    {
        builder.HasIndex(x => x.FromWalletId);
        builder.Property(x => x.Amount)
            .HasColumnType("decimal(18,2)")
            .IsRequired()
            .HasDefaultValue(0);
        
        builder.Property(x => x.Type)
            .IsRequired();

        builder.HasIndex(x => x.Type);

    }
}