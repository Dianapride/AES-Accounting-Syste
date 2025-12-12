using AES.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace AES.Data;

public class AccountingDbContext : DbContext
{
    public DbSet<NuclearFuelBatch> NuclearFuelBatches => Set<NuclearFuelBatch>();
    public DbSet<NuclearMaterialRecord> NuclearMaterialRecords => Set<NuclearMaterialRecord>();

    public AccountingDbContext(DbContextOptions<AccountingDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<NuclearMaterialRecord>(e =>
        {
            e.HasIndex(x => x.MaterialNumber).IsUnique();
        });
    }
}