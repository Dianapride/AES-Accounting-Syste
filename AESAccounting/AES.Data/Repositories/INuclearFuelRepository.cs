using AES.Core.Entities;
using Microsoft.EntityFrameworkCore;
public interface INuclearFuelRepository
{
    Task AddBatchWithRecordAsync(NuclearFuelBatch batch);
    Task<NuclearFuelBatch?> GetByBatchNumberAsync(string batchNumber);
    Task<IEnumerable<NuclearFuelBatch>> GetBatchesInZoneAsync(string zone);
}