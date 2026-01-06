using AeroEngineCatalog.ConsoleApp.Data;
using AeroEngineCatalog.Data;
using AeroEngineCatalog.Interfaces;
using AeroEngineCatalog.Models;
using AeroEngineCatalog.Services;

namespace AeroEngineCatalog;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== Консольный симулятор инженерных расчетов ===");
        Console.WriteLine("Версия с поддержкой сборок (EngineAssembly)\n");
        
        IPartRepository partRepository = new PartRepository();
        IAssemblyRepository assemblyRepository = new AssemblyRepository(partRepository);
        
        var calculationService = new CalculationService(partRepository);
        var assemblyService = new AssemblyService(assemblyRepository, partRepository);
        
        bool exit = false;
        
        while (!exit)
        {
            try
            {
                DisplayMainMenu();
                var choice = Console.ReadLine();
                
                switch (choice)
                {
                    case "1":
                        await PartsMenuAsync(calculationService, partRepository);
                        break;
                        
                    case "2":
                        await AssembliesMenuAsync(assemblyService, assemblyRepository);
                        break;
                        
                    case "3":
                        await assemblyService.DisplayGlobalStatisticsAsync();
                        break;
                        
                    case "0":
                        exit = true;
                        Console.WriteLine("Выход из программы...");
                        break;
                        
                    default:
                        Console.WriteLine("Неверный выбор. Попробуйте снова.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
            
            if (!exit)
            {
                Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                Console.ReadKey();
                Console.Clear();
            }
        }
    }
    
    static void DisplayMainMenu()
    {
        Console.WriteLine("=== ГЛАВНОЕ МЕНЮ ===");
        Console.WriteLine("1. Работа с деталями");
        Console.WriteLine("2. Работа со сборками");
        Console.WriteLine("3. Глобальная статистика");
        Console.WriteLine("0. Выход");
        Console.Write("Выберите опцию: ");
    }
    
    static async Task PartsMenuAsync(CalculationService calculationService, IPartRepository partRepository)
    {
        bool back = false;
        
        while (!back)
        {
            Console.Clear();
            Console.WriteLine("=== РАБОТА С ДЕТАЛЯМИ ===");
            Console.WriteLine("1. Показать все детали");
            Console.WriteLine("2. Найти детали по материалу");
            Console.WriteLine("3. Рассчитать общий вес деталей");
            Console.WriteLine("4. Статистика по материалам");
            Console.WriteLine("5. Добавить новую деталь");
            Console.WriteLine("6. Демонстрация обобщённого репозитория");
            Console.WriteLine("0. Назад");
            Console.Write("Выберите опцию: ");
            
            var choice = Console.ReadLine();
            
            switch (choice)
            {
                case "1":
                    await calculationService.DisplayAllPartsAsync();
                    break;
                    
                case "2":
                    await SearchByMaterialMenuAsync(calculationService);
                    break;
                    
                case "3":
                    await CalculateTotalMassMenuAsync(calculationService);
                    break;
                    
                case "4":
                    await calculationService.DisplayMaterialStatisticsAsync();
                    break;
                    
                case "5":
                    await AddNewPartMenuAsync(partRepository);
                    break;
                    
                case "6":
                    await calculationService.DemonstrateGenericRepositoryAsync();
                    break;
                    
                case "0":
                    back = true;
                    break;
                    
                default:
                    Console.WriteLine("Неверный выбор.");
                    break;
            }
            
            if (!back)
            {
                Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                Console.ReadKey();
            }
        }
    }
    
    static async Task AssembliesMenuAsync(AssemblyService assemblyService, IAssemblyRepository assemblyRepository)
    {
        bool back = false;
        
        while (!back)
        {
            Console.Clear();
            Console.WriteLine("=== РАБОТА СО СБОРКАМИ ===");
            Console.WriteLine("1. Показать все сборки");
            Console.WriteLine("2. Детальная информация о сборке");
            Console.WriteLine("3. Рассчитать массу сборки (асинхронно)");
            Console.WriteLine("4. Статистика по материалам в сборке");
            Console.WriteLine("5. Создать новую сборку");
            Console.WriteLine("6. Поиск сборок по материалу");
            Console.WriteLine("7. Тяжёлые сборки (фильтр по массе)");
            Console.WriteLine("0. Назад");
            Console.Write("Выберите опцию: ");
            
            var choice = Console.ReadLine();
            
            switch (choice)
            {
                case "1":
                    await assemblyService.DisplayAllAssembliesAsync();
                    break;
                    
                case "2":
                    await DisplayAssemblyDetailsMenuAsync(assemblyService);
                    break;
                    
                case "3":
                    await CalculateAssemblyMassMenuAsync(assemblyService);
                    break;
                    
                case "4":
                    await DisplayAssemblyMaterialStatsMenuAsync(assemblyService);
                    break;
                    
                case "5":
                    await assemblyService.CreateNewAssemblyAsync();
                    break;
                    
                case "6":
                    await assemblyService.SearchAssembliesByMaterialAsync();
                    break;
                    
                case "7":
                    await DisplayHeavyAssembliesMenuAsync(assemblyRepository);
                    break;
                    
                case "0":
                    back = true;
                    break;
                    
                default:
                    Console.WriteLine("Неверный выбор.");
                    break;
            }
            
            if (!back)
            {
                Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                Console.ReadKey();
            }
        }
    }
    
    static async Task DisplayAssemblyDetailsMenuAsync(AssemblyService assemblyService)
    {
        Console.Write("Введите ID сборки: ");
        if (int.TryParse(Console.ReadLine(), out int assemblyId))
        {
            await assemblyService.DisplayAssemblyDetailsAsync(assemblyId);
        }
        else
        {
            Console.WriteLine("Неверный ID.");
        }
    }
    
    static async Task CalculateAssemblyMassMenuAsync(AssemblyService assemblyService)
    {
        Console.Write("Введите ID сборки для расчёта массы: ");
        if (int.TryParse(Console.ReadLine(), out int assemblyId))
        {
            await assemblyService.CalculateAndDisplayAssemblyMassAsync(assemblyId);
        }
        else
        {
            Console.WriteLine("Неверный ID.");
        }
    }
    
    static async Task DisplayAssemblyMaterialStatsMenuAsync(AssemblyService assemblyService)
    {
        Console.Write("Введите ID сборки для статистики: ");
        if (int.TryParse(Console.ReadLine(), out int assemblyId))
        {
            await assemblyService.DisplayAssemblyMaterialStatisticsAsync(assemblyId);
        }
        else
        {
            Console.WriteLine("Неверный ID.");
        }
    }
    
    static async Task DisplayHeavyAssembliesMenuAsync(IAssemblyRepository assemblyRepository)
    {
        Console.Write("Введите минимальную массу (кг): ");
        if (double.TryParse(Console.ReadLine(), out double minMass))
        {
            var heavyAssemblies = await assemblyRepository.GetHeavyAssembliesAsync(minMass);
            
            Console.WriteLine($"\nСборки тяжелее {minMass} кг:");
            if (!heavyAssemblies.Any())
            {
                Console.WriteLine("Сборки не найдены.");
                return;
            }
            
            foreach (var assembly in heavyAssemblies)
            {
                Console.WriteLine($"{assembly.Name}: {assembly.EstimatedTotalMass:F2} кг");
            }
        }
        else
        {
            Console.WriteLine("Неверная масса.");
        }
    }
    
    static async Task SearchByMaterialMenuAsync(CalculationService calculationService)
    {
        Console.WriteLine("\n=== Доступные материалы ===");
        foreach (MaterialType material in Enum.GetValues(typeof(MaterialType)))
        {
            Console.WriteLine($"{(int)material}. {material.GetDisplayName()}");
        }
        
        Console.Write("Выберите материал (номер): ");
        if (int.TryParse(Console.ReadLine(), out int materialIndex) &&
            Enum.IsDefined(typeof(MaterialType), materialIndex))
        {
            var material = (MaterialType)materialIndex;
            await calculationService.SearchByMaterialAsync(material);
        }
        else
        {
            Console.WriteLine("Неверный выбор материала.");
        }
    }
    
    static async Task CalculateTotalMassMenuAsync(CalculationService calculationService)
    {
        Console.WriteLine("\nВведите ID деталей через запятую (например: 1,3,5): ");
        var input = Console.ReadLine();
        
        if (!string.IsNullOrWhiteSpace(input))
        {
            try
            {
                var partIds = input.Split(',')
                    .Select(id => int.Parse(id.Trim()))
                    .Distinct();
                
                await calculationService.CalculateAndDisplayTotalMassAsync(partIds);
            }
            catch (FormatException)
            {
                Console.WriteLine("Ошибка: введите числа через запятую!");
            }
        }
    }
    
    static async Task AddNewPartMenuAsync(IPartRepository repository)
    {
        Console.WriteLine("\n=== Добавление новой детали ===");
        
        try
        {
            Console.Write("Название детали: ");
            var name = Console.ReadLine()?.Trim();
            
            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Название не может быть пустым.");
                return;
            }
            
            Console.WriteLine("\nВыберите материал:");
            foreach (MaterialType material in Enum.GetValues(typeof(MaterialType)))
            {
                Console.WriteLine($"{(int)material}. {material.GetDisplayName()}");
            }
            
            Console.Write("Материал (номер): ");
            if (!int.TryParse(Console.ReadLine(), out int materialIndex) ||
                !Enum.IsDefined(typeof(MaterialType), materialIndex))
            {
                Console.WriteLine("Неверный выбор материала.");
                return;
            }
            
            Console.Write("Масса (кг): ");
            if (!double.TryParse(Console.ReadLine(), out double mass) || mass <= 0)
            {
                Console.WriteLine("Неверная масса.");
                return;
            }
            
            Console.Write("Стоимость ($): ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal cost) || cost <= 0)
            {
                Console.WriteLine("Неверная стоимость.");
                return;
            }
            
            var part = new EnginePart
            {
                Name = name,
                Material = (MaterialType)materialIndex,
                Mass = mass,
                Cost = cost,
                DurabilityScore = 100
            };
            
            await repository.AddAsync(part);
            Console.WriteLine($"\n✓ Деталь добавлена с ID: {part.Id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при добавлении: {ex.Message}");
        }
    }
}