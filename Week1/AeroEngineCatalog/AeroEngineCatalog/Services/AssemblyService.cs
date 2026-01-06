using AeroEngineCatalog.Interfaces;
using AeroEngineCatalog.Models;

namespace AeroEngineCatalog.Services;

public class AssemblyService
{
    private readonly IAssemblyRepository _assemblyRepository;
    private readonly IPartRepository _partRepository;
    
    public AssemblyService(IAssemblyRepository assemblyRepository, IPartRepository partRepository)
    {
        _assemblyRepository = assemblyRepository;
        _partRepository = partRepository;
    }
    
    public async Task DisplayAllAssembliesAsync()
    {
        try
        {
            Console.WriteLine("\nВсе сборочные единицы");
            var assemblies = await _assemblyRepository.GetAllAsync();
            
            if (!assemblies.Any())
            {
                Console.WriteLine("Сборки не найдены");
                return;
            }
            
            foreach (var assembly in assemblies)
            {
                Console.WriteLine(assembly);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }
    
    public async Task DisplayAssemblyDetailsAsync(int assemblyId)
    {
        try
        {
            var assembly = await _assemblyRepository.GetByIdAsync(assemblyId);
            if (assembly == null)
            {
                Console.WriteLine($"Сборка с ID {assemblyId} не найдена");
                return;
            }
            
            Console.WriteLine("\n" + assembly.GetDetailedInfo());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }
    
    public async Task CalculateAndDisplayAssemblyMassAsync(int assemblyId)
    {
        try
        {
            var assembly = await _assemblyRepository.GetByIdAsync(assemblyId);
            if (assembly == null)
            {
                Console.WriteLine($"Сборка с ID {assemblyId} не найдена");
                return;
            }
            
            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(60));
            
            Console.WriteLine($"\nРасчёт массы сборки '{assembly.Name}'");
            
            var totalMass = await assembly.CalculateTotalMassAsync(cts.Token);
            
            Console.WriteLine($"\nСборка: {assembly.Name}");
            Console.WriteLine($"Деталей: {assembly.PartCount}");
            Console.WriteLine($"Общая масса: {totalMass:F2} кг");
            Console.WriteLine($"Стоимость: ${assembly.EstimatedTotalCost:F2}");
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("\nРасчёт отменён");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nОшибка: {ex.Message}");
        }
    }
    
    public async Task DisplayAssemblyMaterialStatisticsAsync(int assemblyId)
    {
        try
        {
            var assembly = await _assemblyRepository.GetByIdAsync(assemblyId);
            if (assembly == null)
            {
                Console.WriteLine($"Сборка с ID {assemblyId} не найдена");
                return;
            }
            
            Console.WriteLine($"\nСтатистика по материалам: {assembly.Name}");
            
            var materialStats = assembly.GetMaterialStatistics().ToList();
            
            if (!materialStats.Any())
            {
                Console.WriteLine("В сборке нет деталей");
                return;
            }
            
            Console.WriteLine($"\nИспользуется {materialStats.Count} материалов:");
            
            foreach (var stat in materialStats)
            {
                Console.WriteLine($"\n{stat.MaterialName}:");
                Console.WriteLine($"  Деталей: {stat.PartCount}");
                Console.WriteLine($"  Общая масса: {stat.TotalMass:F2} кг");
                Console.WriteLine($"  Средняя масса: {stat.AverageMass:F2} кг");
                Console.WriteLine($"  Диапазон масс: {stat.MinMass:F2} - {stat.MaxMass:F2} кг");
                Console.WriteLine($"  Общая стоимость: ${stat.TotalCost:F2}");
                Console.WriteLine($"  Средняя прочность: {stat.AverageDurability:F1}%");
            }
            
            Console.WriteLine($"\nСводная статистика");
            Console.WriteLine($"Всего деталей: {assembly.PartCount}");
            Console.WriteLine($"Общая масса: {assembly.EstimatedTotalMass:F2} кг");
            Console.WriteLine($"Средняя масса детали: {assembly.EstimatedTotalMass / assembly.PartCount:F2} кг");
            Console.WriteLine($"Наиболее используемый материал: {materialStats.First().MaterialName}");
            Console.WriteLine($"Наименее используемый материал: {materialStats.Last().MaterialName}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }
    
    public async Task CreateNewAssemblyAsync()
    {
        try
        {
            Console.WriteLine("\nСоздание новой сборки");
            
            Console.Write("Название сборки: ");
            var name = Console.ReadLine()?.Trim();
            
            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Название не может быть пустым");
                return;
            }
            
            Console.Write("Описание: ");
            var description = Console.ReadLine()?.Trim();
            
            var assembly = new EngineAssembly(name, description);
            
            await AddPartsToNewAssemblyAsync(assembly);
            
            await _assemblyRepository.AddAsync(assembly);
            Console.WriteLine($"\nСборка '{name}' создана с ID: {assembly.Id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }
    
    private async Task AddPartsToNewAssemblyAsync(EngineAssembly assembly)
    {
        var addMoreParts = true;
        
        while (addMoreParts)
        {
            Console.WriteLine("\nДобавить деталь в сборку? (y/n)");
            var response = Console.ReadLine()?.Trim().ToLower();
            
            if (response != "y")
            {
                addMoreParts = false;
                continue;
            }
            
            Console.WriteLine("\n1. Выбрать существующую деталь");
            Console.WriteLine("2. Создать новую деталь");
            Console.Write("Выберите опцию: ");
            
            if (int.TryParse(Console.ReadLine(), out int option))
            {
                switch (option)
                {
                    case 1:
                        await AddExistingPartToAssemblyAsync(assembly);
                        break;
                    case 2:
                        await CreateAndAddNewPartToAssemblyAsync(assembly);
                        break;
                    default:
                        Console.WriteLine("Неверный выбор");
                        break;
                }
            }
            
            Console.WriteLine($"\nТекущий состав сборки: {assembly.PartCount} деталей");
        }
    }
    
    private async Task AddExistingPartToAssemblyAsync(EngineAssembly assembly)
    {
        try
        {
            Console.WriteLine("\nДоступные детали");
            var parts = (await _partRepository.GetAllAsync()).ToList();
            
            if (!parts.Any())
            {
                Console.WriteLine("Нет доступных деталей");
                return;
            }
            
            foreach (var part in parts)
            {
                Console.WriteLine($"{part.Id}: {part.Name} ({part.MaterialDisplayName})");
            }
            
            Console.Write("\nВведите ID детали для добавления: ");
            if (int.TryParse(Console.ReadLine(), out int partId))
            {
                var part = await _partRepository.GetByIdAsync(partId);
                if (part != null)
                {
                    assembly.AddPart(part);
                    Console.WriteLine($"Деталь '{part.Name}' добавлена в сборку");
                }
                else
                {
                    Console.WriteLine($"Деталь с ID {partId} не найдена");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }
    
    private async Task CreateAndAddNewPartToAssemblyAsync(EngineAssembly assembly)
    {
        try
        {
            Console.Write("Название детали: ");
            var name = Console.ReadLine()?.Trim();
            
            Console.WriteLine("\nВыберите материал:");
            foreach (MaterialType materialType in Enum.GetValues(typeof(MaterialType)))
            {
                Console.WriteLine($"{(int)materialType}. {materialType.GetDisplayName()}");
            }
            
            Console.Write("Материал (номер): ");
            if (!int.TryParse(Console.ReadLine(), out int materialIndex) ||
                !Enum.IsDefined(typeof(MaterialType), materialIndex))
            {
                Console.WriteLine("Неверный выбор материала");
                return;
            }
            
            var selectedMaterial = (MaterialType)materialIndex;
            
            Console.Write("Масса (кг): ");
            if (!double.TryParse(Console.ReadLine(), out double mass) || mass <= 0)
            {
                Console.WriteLine("Неверная масса");
                return;
            }
            
            Console.Write("Стоимость ($): ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal cost) || cost <= 0)
            {
                Console.WriteLine("Неверная стоимость");
                return;
            }
            
            var part = new EnginePart
            {
                Name = name ?? "Новая деталь",
                Material = selectedMaterial,
                Mass = mass,
                Cost = cost,
                DurabilityScore = 100
            };
            
            await _partRepository.AddAsync(part);
            assembly.AddPart(part);
            
            Console.WriteLine($"Создана и добавлена новая деталь: {part.Name}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }
    
    public async Task SearchAssembliesByMaterialAsync()
    {
        try
        {
            Console.WriteLine("\nПоиск сборок по материалу");
            Console.WriteLine("Выберите материал:");
            
            foreach (MaterialType materialType in Enum.GetValues(typeof(MaterialType)))
            {
                Console.WriteLine($"{(int)materialType}. {materialType.GetDisplayName()}");
            }
            
            Console.Write("Материал (номер): ");
            if (!int.TryParse(Console.ReadLine(), out int materialIndex) ||
                !Enum.IsDefined(typeof(MaterialType), materialIndex))
            {
                Console.WriteLine("Неверный выбор материала");
                return;
            }
            
            var selectedMaterial = (MaterialType)materialIndex;
            var assemblies = await _assemblyRepository.GetByPartMaterialAsync(selectedMaterial);
            
            Console.WriteLine($"\nСборки, содержащие детали из {selectedMaterial.GetDisplayName()}:");
            
            if (!assemblies.Any())
            {
                Console.WriteLine("Сборки не найдены");
                return;
            }
            
            foreach (var assembly in assemblies)
            {
                var partsCount = assembly.Parts.Count(p => p.Material == selectedMaterial);
                Console.WriteLine($"{assembly.Name}: {partsCount} деталей из {selectedMaterial.GetDisplayName()}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }
    
    public async Task DisplayGlobalStatisticsAsync()
    {
        try
        {
            Console.WriteLine("\nГлобальная статистика");
            
            var assemblies = await _assemblyRepository.GetAllAsync();
            var assemblyStats = await _assemblyRepository.GetAssemblyStatisticsAsync();
            var materialUsageStats = await _assemblyRepository.GetMaterialUsageStatisticsAsync();
            
            Console.WriteLine($"\nКоличество сборок: {assemblies.Count()}");
            Console.WriteLine($"Общее количество деталей: {assemblies.Sum(a => a.PartCount)}");
            
            if (assemblies.Any())
            {
                Console.WriteLine($"\nСтатистика по сборкам");
                foreach (var stat in assemblyStats)
                {
                    Console.WriteLine(stat);
                }
                
                Console.WriteLine($"\nИспользование материалов");
                foreach (var kvp in materialUsageStats.OrderByDescending(kv => kv.Value))
                {
                    Console.WriteLine($"{kvp.Key.GetDisplayName()}: {kvp.Value} деталей");
                }
                
                var allParts = assemblies.SelectMany(a => a.Parts);
                
                Console.WriteLine($"\nАнализ по материалам");
                var materialAnalysis = allParts
                    .GroupBy(p => p.Material)
                    .Select(g => new
                    {
                        MaterialName = g.Key.GetDisplayName(),
                        Count = g.Count(),
                        AvgMass = g.Average(p => p.Mass),
                        TotalCost = g.Sum(p => p.Cost),
                        AssembliesCount = g.Select(p => p).Count()
                    })
                    .OrderByDescending(m => m.Count);
                
                foreach (var analysis in materialAnalysis)
                {
                    Console.WriteLine($"{analysis.MaterialName}:");
                    Console.WriteLine($"  Используется в {analysis.Count} деталях");
                    Console.WriteLine($"  Средняя масса: {analysis.AvgMass:F2} кг");
                    Console.WriteLine($"  Общая стоимость: ${analysis.TotalCost:F2}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }
}