namespace AeroEngineCatalog.Models;

public enum MaterialType
{
    Titanium = 1,
    Composite = 2,
    Steel = 3,
    Aluminum = 4,
    NickelAlloy = 5,
    Superalloy = 6,
    Ceramic = 7,
    StainlessSteel = 8,
    TitaniumAlloy = 9,
    CarbonFiber = 10
}

public static class MaterialTypeExtensions
{
    public static string GetDisplayName(this MaterialType material)
    {
        return material switch
        {
            MaterialType.Titanium => "Титан",
            MaterialType.Composite => "Композит",
            MaterialType.Steel => "Сталь",
            MaterialType.Aluminum => "Алюминий",
            MaterialType.NickelAlloy => "Никелевый сплав",
            MaterialType.Superalloy => "Жаропрочный сплав",
            MaterialType.Ceramic => "Керамика",
            MaterialType.StainlessSteel => "Нержавеющая сталь",
            MaterialType.TitaniumAlloy => "Титановый сплав",
            MaterialType.CarbonFiber => "Углеволокно",
            _ => material.ToString()
        };
    }
}

public class EnginePart
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public MaterialType Material { get; set; }
    public double Mass { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    public string? Manufacturer { get; set; }
    public decimal Cost { get; set; }
    public int DurabilityScore { get; set; } = 100;
    
    public string MaterialDisplayName => Material.GetDisplayName();
    
    public override string ToString()
    {
        return $"{Id}: {Name} | {MaterialDisplayName} | {Mass:F2} кг | ${Cost:F2} | Прочность: {DurabilityScore}%";
    }
}