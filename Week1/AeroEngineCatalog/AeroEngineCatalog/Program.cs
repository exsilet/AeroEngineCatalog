using AeroEngineCatalog.Data;
using AeroEngineCatalog.Interfaces;
using AeroEngineCatalog.Services;

namespace AeroEngineCatalog;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== Консольный симулятор инженерных расчетов ===\n");
        
        // Инициализация зависимостей (простая версия, без DI контейнера)
        IPartRepository repository = new PartRepository();
        var calculationService = new CalculationService(repository);
        
        bool exit = false;
        
        while (!exit)
        {
            DisplayMenu();
            var choice = Console.ReadLine();
            
            switch (choice)
            {
                case "1":
                    DisplayAllParts(repository);
                    break;
                    
                case "2":
                    SearchByMaterial(calculationService);
                    break;
                    
                case "3":
                    await CalculateTotalMassAsync(calculationService);
                    break;
                    
                case "4":
                    calculationService.DisplayMaterialStatistics();
                    break;
                    
                case "5":
                    AddNewPart(repository);
                    break;
                    
                case "0":
                    exit = true;
                    Console.WriteLine("Выход из программы...");
                    break;
                    
                default:
                    Console.WriteLine("Неверный выбор. Попробуйте снова.");
                    break;
            }
            
            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey();
            Console.Clear();
        }
    }
    
    static void DisplayMenu()
    {
        Console.WriteLine("Меню:");
        Console.WriteLine("1. Показать все детали");
        Console.WriteLine("2. Найти детали по материалу");
        Console.WriteLine("3. Рассчитать общий вес деталей");
        Console.WriteLine("4. Показать статистику по материалам");
        Console.WriteLine("5. Добавить новую деталь");
        Console.WriteLine("0. Выход");
        Console.Write("Выберите опцию: ");
    }
    
    static void DisplayAllParts(IPartRepository repository)
    {
        Console.WriteLine("\n=== Все детали двигателя ===");
        foreach (var part in repository.GetAll())
        {
            Console.WriteLine(part);
        }
    }
    
    static void SearchByMaterial(CalculationService service)
    {
        Console.Write("Введите материал для поиска: ");
        var material = Console.ReadLine();
        
        if (!string.IsNullOrWhiteSpace(material))
        {
            var parts = service.FindPartsByMaterial(material);
            
            Console.WriteLine($"\nНайдено {parts.Count()} деталей:");
            foreach (var part in parts)
            {
                Console.WriteLine($"  {part}");
            }
        }
    }
    
    static async Task CalculateTotalMassAsync(CalculationService service)
    {
        Console.WriteLine("Введите ID деталей через запятую (например: 1,3,5): ");
        var input = Console.ReadLine();
        
        if (!string.IsNullOrWhiteSpace(input))
        {
            try
            {
                var partIds = input.Split(',')
                    .Select(id => int.Parse(id.Trim()))
                    .ToList();
                
                Console.WriteLine("Выполняется расчёт...");
                var totalMass = await service.CalculateTotalMassAsync(partIds);
                
                Console.WriteLine($"\nОбщий вес выбранных деталей: {totalMass:F2} кг");
            }
            catch (FormatException)
            {
                Console.WriteLine("Ошибка: введите числа через запятую!");
            }
        }
    }
    
    static void AddNewPart(IPartRepository repository)
    {
        Console.WriteLine("\n=== Добавление новой детали ===");
        
        try
        {
            Console.Write("Название детали: ");
            var name = Console.ReadLine();
            
            Console.Write("Материал: ");
            var material = Console.ReadLine();
            
            Console.Write("Масса (кг): ");
            var mass = double.Parse(Console.ReadLine() ?? "0");
            
            var part = new Models.EnginePart
            {
                Name = name ?? "Неизвестная деталь",
                Material = material ?? "Неизвестный материал",
                Mass = mass
            };
            
            repository.Add(part);
            Console.WriteLine($"Деталь добавлена с ID: {part.Id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при добавлении: {ex.Message}");
        }
    }
}