namespace AES.Core.Entities;

public class NuclearMaterialRecord
{
    public int Id { get; set; }
    public int BatchId { get; set; }
    public NuclearFuelBatch Batch { get; set; } = null!;

    public string MaterialNumber { get; set; } = null!;       // например ЯМ-20251211-ТВЗ123
    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Зареєстровано";
}