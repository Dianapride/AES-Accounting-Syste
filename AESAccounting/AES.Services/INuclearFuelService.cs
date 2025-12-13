using AES.Core.Entities;

namespace AES.Services;
public interface INuclearFuelService
{
    Task<OperationResult> RegisterFuelBatchAsync(NuclearFuelBatch batch);
    Task<NuclearFuelBatch?> GetBatchByNumberAsync(string batchNumber);
}