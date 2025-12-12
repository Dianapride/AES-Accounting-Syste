using AES.Data;
using AES.Core.Entities;
using Microsoft.EntityFrameworkCore;
public class NuclearFuelRepository : INuclearFuelRepository
{
    private readonly AccountingDbContext _ctx;
    public NuclearFuelRepository(AccountingDbContext ctx) => _ctx = ctx;

    public async Task AddBatchWithRecordAsync(NuclearFuelBatch batch)
    {
        var record = new NuclearMaterialRecord
        {
            MaterialNumber = $"ЯМ-{DateTime.Now:yyyyMMdd}-{batch.BatchNumber}",
            RegistrationDate = DateTime.UtcNow,
            Status = "Зареєстровано"
        };
        batch.MaterialRecords.Add(record);

        _ctx.NuclearFuelBatches.Add(batch);
        await _ctx.SaveChangesAsync();
    }

    public async Task<NuclearFuelBatch?> GetByBatchNumberAsync(string batchNumber) =>
        await _ctx.NuclearFuelBatches
                  .Include(b => b.MaterialRecords)
                  .FirstOrDefaultAsync(b => b.BatchNumber == batchNumber);

    public async Task<IEnumerable<NuclearFuelBatch>> GetBatchesInZoneAsync(string zone) =>
        await _ctx.NuclearFuelBatches
                  .Where(b => b.StorageZone == zone)
                  .ToListAsync();
}