namespace Payphone.Infrastructure.EF.Persistence;

public class ApplicationDbContext : IdentityDbContext<User>
{
    private readonly IApplicationContext _appContext;
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IApplicationContext appContext) :base(options)
    {
        _appContext = appContext;
        
        if (Database.GetPendingMigrations().Any())
        {
            Database.Migrate();
        } 
    }
    
    public DbSet<Wallet> Wallets { get; set; }
    public DbSet<WalletTransaction> WalletTransactions { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
    
    
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entity in ChangeTracker.Entries<BaseModel>())
        {
            switch (entity.State)
            {
                case EntityState.Added:
                    entity.Entity.CreatedAt = DateTime.UtcNow;
                    entity.Entity.CreatedBy = _appContext.UserId;
                    break;
                case EntityState.Modified:
                    entity.Entity.UpdatedAt = DateTime.UtcNow;
                    entity.Entity.UpdatedBy = _appContext.UserId;
                    break;
                    
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}