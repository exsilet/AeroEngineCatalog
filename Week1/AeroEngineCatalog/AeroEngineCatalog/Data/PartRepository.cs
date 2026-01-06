using System.Linq.Expressions;
using AeroEngineCatalog.Interfaces;
using AeroEngineCatalog.Models;

namespace AeroEngineCatalog.Data;

public class PartRepository : IPartRepository
{
    private readonly List<EnginePart> _parts = new();
    private int _nextId = 1;

    public PartRepository()
    {
        InitializeDefaultPartsAsync().Wait();
    }

    private async Task InitializeDefaultPartsAsync()
    {
        await AddRangeAsync(new List<EnginePart>
        {
            new()
            {
                Name = "Лопатка турбины",
                Material = MaterialType.Titanium,
                Mass = 2.5,
                Manufacturer = "Pratt & Whitney",
                Cost = 12500.50m,
                DurabilityScore = 95
            },
            new()
            {
                Name = "Диск компрессора",
                Material = MaterialType.NickelAlloy,
                Mass = 15.8,
                Manufacturer = "GE Aviation",
                Cost = 32500.75m,
                DurabilityScore = 98
            },
            new()
            {
                Name = "Камера сгорания",
                Material = MaterialType.Superalloy,
                Mass = 32.1,
                Manufacturer = "Rolls-Royce",
                Cost = 45000.00m,
                DurabilityScore = 96
            },
            new()
            {
                Name = "Лопатка вентилятора",
                Material = MaterialType.TitaniumAlloy,
                Mass = 3.2,
                Manufacturer = "Safran",
                Cost = 18500.25m,
                DurabilityScore = 97
            },
            new()
            {
                Name = "Направляющий аппарат",
                Material = MaterialType.Ceramic,
                Mass = 1.8,
                Manufacturer = "Honeywell",
                Cost = 9500.00m,
                DurabilityScore = 99
            }
        });

        Console.WriteLine($"Репозиторий инициализирован с {_parts.Count} деталями");
    }

    public async Task<EnginePart?> GetByIdAsync(int id)
    {
        await Task.Delay(10);
        return _parts.FirstOrDefault(p => p.Id == id);
    }

    public async Task<IEnumerable<EnginePart>> GetAllAsync()
    {
        await Task.Delay(10);
        return _parts.AsReadOnly();
    }

    public async Task<IEnumerable<EnginePart>> FindAsync(Expression<Func<EnginePart, bool>> predicate)
    {
        await Task.Delay(10);
        return _parts.AsQueryable().Where(predicate).ToList();
    }

    public async Task AddAsync(EnginePart part)
    {
        await Task.Delay(10);

        if (part == null)
            throw new ArgumentNullException(nameof(part));

        part.Id = _nextId++;
        part.CreatedAt = DateTime.UtcNow;
        _parts.Add(part);

        Console.WriteLine($"Добавлена деталь: {part.Name} (ID: {part.Id})");
    }

    public async Task AddRangeAsync(IEnumerable<EnginePart> parts)
    {
        foreach (var part in parts)
        {
            await AddAsync(part);
        }
    }

    public async Task UpdateAsync(EnginePart part)
    {
        await Task.Delay(10);

        var existingPart = await GetByIdAsync(part.Id);
        if (existingPart == null)
            throw new KeyNotFoundException($"Деталь с ID {part.Id} не найдена");

        part.UpdatedAt = DateTime.UtcNow;

        var index = _parts.FindIndex(p => p.Id == part.Id);
        if (index >= 0)
        {
            _parts[index] = part;
        }

        Console.WriteLine($"Обновлена деталь ID: {part.Id}");
    }

    public async Task RemoveAsync(EnginePart part)
    {
        await Task.Delay(10);
        _parts.Remove(part);
        Console.WriteLine($"Удалена деталь ID: {part.Id}");
    }

    public async Task<bool> ExistsAsync(int id)
    {
        var part = await GetByIdAsync(id);
        return part != null;
    }

    public async Task<int> CountAsync()
    {
        await Task.Delay(10);
        return _parts.Count;
    }

    public async Task<IEnumerable<EnginePart>> GetByMaterialAsync(MaterialType material)
    {
        return await FindAsync(p => p.Material == material);
    }

    public async Task<IEnumerable<EnginePart>> GetHeavierThanAsync(double minMass)
    {
        var parts = await FindAsync(p => p.Mass > minMass);
        return parts.OrderByDescending(p => p.Mass);
    }

    public async Task<IEnumerable<EnginePart>> GetLighterThanAsync(double maxMass)
    {
        var parts = await FindAsync(p => p.Mass < maxMass);
        return parts.OrderBy(p => p.Mass);
    }

    public async Task<double> GetTotalMassAsync()
    {
        var parts = await GetAllAsync();
        return parts.Sum(p => p.Mass);
    }

    public async Task<double> CalculateTotalMassAsync(IEnumerable<int> partIds)
    {
        Console.WriteLine($"Начинаем расчёт массы для {partIds.Count()} деталей...");

        double totalMass = 0;

        // Оптимизация: работаем с IEnumerable без создания лишних списков
        foreach (var partId in partIds)
        {
            var part = await GetByIdAsync(partId);
            if (part != null)
            {
                totalMass += part.Mass;
                Console.WriteLine($"  Добавлена масса детали {part.Name}: {part.Mass} кг");
            }
            else
            {
                Console.WriteLine($"  ⚠ Деталь с ID {partId} не найдена");
            }

            await Task.Delay(100);
        }

        return totalMass;
    }
}