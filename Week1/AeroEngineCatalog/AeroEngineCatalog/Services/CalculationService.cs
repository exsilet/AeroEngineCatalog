using AeroEngineCatalog.Interfaces;
using AeroEngineCatalog.Models;

namespace AeroEngineCatalog.Services;

public class CalculationService
{
    private readonly IPartRepository  _partRepository;

    public CalculationService(IPartRepository partRepository)
    {
       _partRepository = partRepository;
    }

    public IEnumerable<EnginePart> FindPartsByMaterial(string material)
    {
        Console.WriteLine($"Поиск деталей из материала: {material}");
        return _partRepository.GetByMaterial(material);
    }
    
    public async Task<double> CalculateTotalMassAsync(IEnumerable<int> partIds)
    {
        Console.WriteLine("Начинаем расчёт общего веса...");
        
        await Task.Delay(1000);
        
        var parts = _partRepository.GetAll()
            .Where(p => partIds.Contains(p.Id))
            .ToList();
        
        double totalMass = parts.Sum(p => p.Mass);
        
        Console.WriteLine($"Рассчитано: {parts.Count} деталей");
        return totalMass;
    }
    
    public void DisplayMaterialStatistics()
    {
        var stats = _partRepository.GetAll()
            .GroupBy(p => p.Material)
            .Select(g => new
            {
                Material = g.Key,
                Count = g.Count(),
                TotalMass = g.Sum(p => p.Mass),
                AverageMass = g.Average(p => p.Mass)
            })
            .OrderByDescending(s => s.TotalMass);
        
        Console.WriteLine("\n=== Статистика по материалам ===");
        
        foreach (var stat in stats)
        {
            Console.WriteLine($"Материал: {stat.Material}");
            Console.WriteLine($"  Количество деталей: {stat.Count}");
            Console.WriteLine($"  Общая масса: {stat.TotalMass:F2} кг");
            Console.WriteLine($"  Средняя масса: {stat.AverageMass:F2} кг\n");
        }
    }
}