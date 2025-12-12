using AES.Core.Entities;
using AES.Data;
using Microsoft.EntityFrameworkCore;

// Налаштування підключення до локальної тестової SQLite-бази
var options = new DbContextOptionsBuilder<AccountingDbContext>()
    .UseSqlite("Data Source=aes_test.db")
    .Options;

// Створюємо контекст і базу (якщо ще немає)
using var context = new AccountingDbContext(options);
await context.Database.EnsureCreatedAsync();

Console.WriteLine("База створена / відкрита успішно.");
Console.WriteLine();

// Створюємо тестову партію ТВЗ
var newBatch = new NuclearFuelBatch
{
    BatchNumber = "ТВЗ-45871",
    ReceiptDate = DateTime.UtcNow,
    UraniumMassKg = 485.2m,
    EnrichmentPercent = 4.95m,
    StorageZone = "Зона-1А",
    IsotopeComposition = "U-235: 4.95%, U-238: 95.05%"
};

// Використовуємо наш репозиторій
var repository = new NuclearFuelRepository(context);
await repository.AddBatchWithRecordAsync(newBatch);

Console.WriteLine("УСПІШНО!");
Console.WriteLine($"Додано партію ТВЗ №{newBatch.BatchNumber}");
Console.WriteLine($"Автоматично створено запис ЯМ з номером: ЯМ-{DateTime.Now:yyyyMMdd}-ТВЗ-45871");
Console.WriteLine();
Console.WriteLine("Файл бази створено: aes_test.db (можна відкрити в DB Browser for SQLite)");

// Додатково — покажемо, що можемо читати назад
var loaded = await repository.GetByBatchNumberAsync("ТВЗ-45871");
Console.WriteLine($"Зчитано з бази: {loaded?.BatchNumber} – записів ЯМ: {loaded?.MaterialRecords.Count}");

Console.WriteLine("\nНатисни будь-яку клавішу для виходу...");
Console.ReadKey();