using AeroEngineCatalog.Interfaces;
using AeroEngineCatalog.Models;

namespace AeroEngineCatalog.Services;

public class CalculationService
{
    private readonly IPartRepository _repository;
    
    public CalculationService(IPartRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }
    
    public async Task DisplayAllPartsAsync()
    {
        try
        {
            Console.WriteLine("\n=== Все детали двигателя ===");
            var parts = await _repository.GetAllAsync();
            
            foreach (var part in parts)
            {
                Console.WriteLine(part);
            }
            
            Console.WriteLine($"\nВсего деталей: {parts.Count()}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при получении деталей: {ex.Message}");
        }
    }
    
    public async Task SearchByMaterialAsync(MaterialType material)
    {
        try
        {
            Console.WriteLine($"\nПоиск деталей из материала: {material.GetDisplayName()}");
            
            var parts = await _repository.GetByMaterialAsync(material);
            
            if (!parts.Any())
            {
                Console.WriteLine($"Детали из материала {material.GetDisplayName()} не найдены.");
                return;
            }
            
            Console.WriteLine($"Найдено {parts.Count()} деталей:");
            foreach (var part in parts)
            {
                Console.WriteLine($"  {part}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при поиске: {ex.Message}");
        }
    }
    
    public async Task CalculateAndDisplayTotalMassAsync(IEnumerable<int> partIds)
    {
        try
        {
            Console.WriteLine("\nНачинаем расчёт общей массы...");
            
            var totalMass = await _repository.CalculateTotalMassAsync(partIds);
            
            Console.WriteLine($"\n✓ Расчёт завершён!");
            Console.WriteLine($"Общий вес выбранных деталей: {totalMass:F2} кг");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n✗ Ошибка при расчёте: {ex.Message}");
        }
    }
    
    public async Task DisplayMaterialStatisticsAsync()
    {
        try
        {
            var parts = await _repository.GetAllAsync();
            var materialGroups = parts
                .GroupBy(p => p.Material)
                .Select(g => new
                {
                    Material = g.Key,
                    DisplayName = g.Key.GetDisplayName(),
                    Count = g.Count(),
                    TotalMass = g.Sum(p => p.Mass),
                    AverageMass = g.Average(p => p.Mass),
                    TotalCost = g.Sum(p => p.Cost)
                })
                .OrderByDescending(s => s.TotalMass);
            
            Console.WriteLine("\n=== Статистика по материалам ===");
            foreach (var stat in materialGroups)
            {
                Console.WriteLine($"\nМатериал: {stat.DisplayName}");
                Console.WriteLine($"  Количество деталей: {stat.Count}");
                Console.WriteLine($"  Общая масса: {stat.TotalMass:F2} кг");
                Console.WriteLine($"  Средняя масса: {stat.AverageMass:F2} кг");
                Console.WriteLine($"  Общая стоимость: ${stat.TotalCost:F2}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при получении статистики: {ex.Message}");
        }
    }
    
    public async Task DemonstrateGenericRepositoryAsync()
    {
        Console.WriteLine("\n=== Демонстрация обобщённого репозитория ===");
        
        try
        {
            // Используем IRepository<T> через IPartRepository
            IRepository<EnginePart> genericRepository = _repository;
            
            // Пример 1: Получение по ID
            Console.WriteLine("\nПример 1: Получение детали по ID");
            var part = await genericRepository.GetByIdAsync(1);
            if (part != null)
            {
                Console.WriteLine($"  Найдена: {part.Name}");
            }
            
            // Пример 2: Подсчёт
            Console.WriteLine("\nПример 2: Подсчёт деталей");
            var count = await genericRepository.CountAsync();
            Console.WriteLine($"  Всего деталей: {count}");
            
            // Пример 3: Поиск с LINQ выражением
            Console.WriteLine("\nПример 3: Поиск дорогих деталей");
            var expensiveParts = await genericRepository.FindAsync(p => p.Cost > 20000);
            Console.WriteLine($"  Деталей дороже $20000: {expensiveParts.Count()}");
            
            foreach (var expensivePart in expensiveParts)
            {
                Console.WriteLine($"    {expensivePart.Name} - ${expensivePart.Cost}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при демонстрации: {ex.Message}");
        }
    }
}