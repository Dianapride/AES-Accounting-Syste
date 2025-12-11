namespace AES.Core.Entities;

public class NuclearFuelBatch
{
    public int Id { get; set; }
    public string BatchNumber { get; set; } = null!;           // № ТВЗ
    public DateTime ReceiptDate { get; set; } = DateTime.UtcNow;
    public decimal UraniumMassKg { get; set; }
    public decimal EnrichmentPercent { get; set; }
    public string StorageZone { get; set; } = null!;
    public string IsotopeComposition { get; set; } = null!;

    public List<NuclearMaterialRecord> MaterialRecords { get; set; } = new();
}