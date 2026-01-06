using System.Text;

namespace AeroEngineCatalog.Models;

public class EngineAssembly
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<EnginePart> Parts { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public int PartCount => Parts.Count;

    public double EstimatedTotalMass
    {
        get
        {
            if (!_totalMassCache.HasValue)
            {
                _totalMassCache = Parts.Sum(p => p.Mass);
            }

            return _totalMassCache.Value;
        }
    }

    public decimal EstimatedTotalCost => Parts.Sum(p => p.Cost);
    public double AverageDurability => Parts.Any() ? Parts.Average(p => p.DurabilityScore) : 0;

    private double? _totalMassCache;

    // Конструкторы
    public EngineAssembly() { }

    public EngineAssembly(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public EngineAssembly(string name, string description, IEnumerable<EnginePart> parts)
        : this(name, description)
    {
        Parts.AddRange(parts);
    }

    public void AddPart(EnginePart part)
    {
        if (part == null)
            throw new ArgumentNullException(nameof(part));

        Parts.Add(part);
        _totalMassCache = null;
    }

    public void AddParts(IEnumerable<EnginePart> parts)
    {
        foreach (var part in parts)
        {
            AddPart(part);
        }
    }

    public bool RemovePart(int partId)
    {
        var part = Parts.FirstOrDefault(p => p.Id == partId);
        if (part != null)
        {
            Parts.Remove(part);
            _totalMassCache = null;
            return true;
        }

        return false;
    }

    public bool ContainsPart(int partId) => Parts.Any(p => p.Id == partId);

    public EnginePart? FindPart(string partName) =>
        Parts.FirstOrDefault(p => p.Name.Contains(partName, StringComparison.OrdinalIgnoreCase));

    public async Task<double> CalculateTotalMassAsync(CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"Начинаем расчёт массы сборки '{Name}'...");

        // Имитация долгой операции для демонстрации async
        await Task.Delay(500, cancellationToken);

        double totalMass = 0;
        int processedCount = 0;

        foreach (var part in Parts)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                Console.WriteLine("Расчёт отменён.");
                return totalMass;
            }

            // Имитация сложного расчёта для каждой детали
            await Task.Delay(50, cancellationToken);

            totalMass += part.Mass;
            processedCount++;

            if (Parts.Count > 0 && processedCount % Math.Max(1, Parts.Count / 4) == 0)
            {
                Console.WriteLine($"  Прогресс: {processedCount}/{Parts.Count} деталей обработано...");
            }
        }

        _totalMassCache = totalMass;

        Console.WriteLine($"✓ Расчёт завершён! Обработано {processedCount} деталей.");
        return totalMass;
    }

    public Dictionary<MaterialType, List<EnginePart>> GroupPartsByMaterial()
    {
        return Parts
            .GroupBy(p => p.Material)
            .ToDictionary(
                g => g.Key,
                g => g.ToList()
            );
    }

    public IEnumerable<MaterialStatistics> GetMaterialStatistics()
    {
        return Parts
            .GroupBy(p => p.Material)
            .Select(g => new MaterialStatistics
            {
                Material = g.Key,
                MaterialName = g.Key.GetDisplayName(),
                PartCount = g.Count(),
                TotalMass = g.Sum(p => p.Mass),
                AverageMass = g.Average(p => p.Mass),
                TotalCost = g.Sum(p => p.Cost),
                AverageDurability = g.Average(p => p.DurabilityScore),
                MinMass = g.Min(p => p.Mass),
                MaxMass = g.Max(p => p.Mass)
            })
            .OrderByDescending(s => s.TotalMass);
    }

    public IEnumerable<EnginePart> GetPartsByMaterial(MaterialType material) =>
        Parts.Where(p => p.Material == material);

    public IEnumerable<EnginePart> GetHeaviestParts(int count = 5) =>
        Parts.OrderByDescending(p => p.Mass).Take(count);

    public IEnumerable<EnginePart> GetMostExpensiveParts(int count = 5) =>
        Parts.OrderByDescending(p => p.Cost).Take(count);

    public override string ToString()
    {
        return
            $"[{Id}] {Name} | Деталей: {PartCount} | Масса: {EstimatedTotalMass:F2} кг | Стоимость: ${EstimatedTotalCost:F2}";
    }

    public string GetDetailedInfo()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Сборка: {Name}");
        sb.AppendLine($"Описание: {Description}");
        sb.AppendLine($"Количество деталей: {PartCount}");
        sb.AppendLine($"Общая масса: {EstimatedTotalMass:F2} кг");
        sb.AppendLine($"Общая стоимость: ${EstimatedTotalCost:F2}");
        sb.AppendLine($"Средняя прочность: {AverageDurability:F1}%");
        sb.AppendLine($"Создана: {CreatedAt:dd.MM.yyyy HH:mm}");

        if (Parts.Any())
        {
            sb.AppendLine("\nСостав сборки:");
            foreach (var part in Parts.OrderBy(p => p.Name))
            {
                sb.AppendLine($"  • {part.Name} ({part.MaterialDisplayName}) - {part.Mass} кг");
            }
        }

        return sb.ToString();
    }
}

public class MaterialStatistics
{
    public MaterialType Material { get; set; }
    public string MaterialName { get; set; } = string.Empty;
    public int PartCount { get; set; }
    public double TotalMass { get; set; }
    public double AverageMass { get; set; }
    public decimal TotalCost { get; set; }
    public double AverageDurability { get; set; }
    public double MinMass { get; set; }
    public double MaxMass { get; set; }

    public override string ToString()
    {
        return $"{MaterialName}: {PartCount} деталей, Масса: {TotalMass:F2} кг, Стоимость: ${TotalCost:F2}";
    }
}