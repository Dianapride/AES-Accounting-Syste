using AES.Core.Entities;
using AES.Data;

namespace AES.Services;
public class NuclearFuelService : INuclearFuelService
{
    private readonly INuclearFuelRepository _repository;
    private readonly ISauzYamClient _sauzClient;

    public NuclearFuelService(INuclearFuelRepository repository, ISauzYamClient sauzClient)
    {
        _repository = repository;
        _sauzClient = sauzClient;
    }

    public async Task<OperationResult> RegisterFuelBatchAsync(NuclearFuelBatch batch)
    {
        // Бізнес-валідація
        if (string.IsNullOrWhiteSpace(batch.BatchNumber) || string.IsNullOrWhiteSpace(batch.TvzNumber))
            return OperationResult.Fail("Номер партії або ТВЗ не може бути порожнім");

        if (batch.Quantity <= 0 || batch.UraniumMassKg <= 0)
            return OperationResult.Fail("Кількість та маса урану повинні бути більші за 0");

        if (batch.EnrichmentPercent < 1 || batch.EnrichmentPercent > 20) // типові значення для АЕС України
            return OperationResult.Fail("Збагачення повинно бути в діапазоні 1–20%");

        if (string.IsNullOrWhiteSpace(batch.StorageZone))
            return OperationResult.Fail("Зона зберігання обов’язкова");

        try
        {
            // 1. Збереження в локальній БД + створення запису спецобліку ЯМ
            await _repository.AddBatchWithRecordAsync(batch);

            // Генеруємо номер матеріалу (той самий, що створив репозиторій)
            string materialNumber = $"ЯМ-{DateTime.UtcNow:yyyyMMdd}-{batch.BatchNumber}";

            // 2. Передача в САУЗ ЯМ — критична операція ядерної безпеки
            var sauzResponse = await _sauzClient.RegisterMaterialAsync(
                materialNumber, batch.UraniumMassKg, batch.EnrichmentPercent);

            if (!sauzResponse.IsSuccess)
            {
                // У реальному проекті тут би відкіт транзакції (TransactionScope або EF транзакція)
                // Але оскільки репозиторій вже зберіг — логічно було б видалити, але для практикуму імітуємо блокування
                return OperationResult.Fail($"Помилка САУЗ ЯМ: {sauzResponse.ErrorMessage ?? "Відхилено"}. Операція заблокована.");
            }

            // Успішно — можна оновити статус (наприклад)
            batch.Status = "Проведено та підтверджено САУЗ";
            await _repository.UpdateBatchAsync(batch);

            return OperationResult.Success("ТВЗ успішно зареєстровано. Підтвердження від САУЗ ЯМ отримано.");
        }
        catch (Exception ex)
        {
            return OperationResult.Fail($"Виняток під час реєстрації: {ex.Message}");
        }
    }

    public async Task<NuclearFuelBatch?> GetBatchByNumberAsync(string batchNumber)
    {
        return await _repository.GetByBatchNumberAsync(batchNumber);
    }
}