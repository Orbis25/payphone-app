namespace Payphone.Infrastructure.EF.Configurations;

public class WalletEfConfiguration : EfCoreConfiguration<Wallet>
{
    protected override void ConfigureEf(EntityTypeBuilder<Wallet> builder)
    {
        builder.HasIndex(x => x.WalletCode);
        builder.Property(x => x.WalletCode)
            .IsRequired()
            .Metadata
            .SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

        builder.Property(x => x.OwnerDocumentId)
            .IsRequired()
            .HasMaxLength(20);
        
        builder.Property(x => x.OwnerName)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(x => x.OwnerDocumentId);
        builder.HasIndex(x => new { x.OwnerDocumentId, x.WalletCode })
            .IsUnique();
        
        builder.Property(x => x.CurrentBalance)
            .HasColumnType("decimal(18,2)")
            .IsRequired()
            .HasDefaultValue(0);
        
        builder.HasMany(x => x.Transactions)
            .WithOne(x => x.Wallet)
            .HasForeignKey(x => x.WalletId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}