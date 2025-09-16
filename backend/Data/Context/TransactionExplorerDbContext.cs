using Microsoft.EntityFrameworkCore;
using Data.Models;

namespace Data.Context;

public class TransactionExplorerDbContext : DbContext
{
    public TransactionExplorerDbContext(DbContextOptions<TransactionExplorerDbContext> options)
        : base(options)
    {
    }

    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<ExchangeRate> ExchangeRates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Transaction entity
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.CustomId).IsUnique();

            entity.Property(e => e.Id)
                .UseIdentityColumn();

            entity.Property(e => e.CustomId)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.Description)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.TransactionDate)
                .HasColumnType("date")
                .IsRequired();

            entity.Property(e => e.PurchaseAmount)
                .HasColumnType("decimal(19,2)")
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetimeoffset")
                .HasDefaultValueSql("SYSDATETIMEOFFSET()");

            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetimeoffset")
                .HasDefaultValueSql("SYSDATETIMEOFFSET()");

            // Add check constraint for positive purchase amount
            entity.ToTable(t => t.HasCheckConstraint("CK_Transactions_PurchaseAmount", "PurchaseAmount > 0"));
        });

        // Configure ExchangeRate entity
        modelBuilder.Entity<ExchangeRate>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .UseIdentityColumn();

            entity.Property(e => e.RecordDate)
                .HasColumnType("date")
                .IsRequired();

            entity.Property(e => e.CountryCurrencyDesc)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.ExchangeRateValue)
                .HasColumnType("decimal(19,6)")
                .HasColumnName("ExchangeRate")
                .IsRequired();

            entity.Property(e => e.EffectiveDate)
                .HasColumnType("date")
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetimeoffset")
                .HasDefaultValueSql("SYSDATETIMEOFFSET()");

            // Create indexes for efficient lookups
            entity.HasIndex(e => new { e.CountryCurrencyDesc, e.EffectiveDate })
                .HasDatabaseName("IX_ExchangeRates_Currency_Date");

            entity.HasIndex(e => e.EffectiveDate)
                .HasDatabaseName("IX_ExchangeRates_EffectiveDate");
        });
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is Transaction &&
                       (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entityEntry in entries)
        {
            var entity = (Transaction)entityEntry.Entity;

            if (entityEntry.State == EntityState.Added)
            {
                entity.CreatedAt = DateTimeOffset.UtcNow;
            }

            entity.UpdatedAt = DateTimeOffset.UtcNow;
        }
    }
}
