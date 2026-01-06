using AeroEngineCatalog.Interfaces;
using AeroEngineCatalog.Models;

namespace AeroEngineCatalog.ConsoleApp.Data;

public class AssemblyRepository : IAssemblyRepository
{
    private readonly List<EngineAssembly> _assemblies = new();
    private readonly IPartRepository _partRepository;
    private int _nextId = 1;
    
    public AssemblyRepository(IPartRepository partRepository)
    {
        _partRepository = partRepository ?? throw new ArgumentNullException(nameof(partRepository));
        InitializeDefaultAssembliesAsync().Wait();
    }
    
    private async Task InitializeDefaultAssembliesAsync()
    {
        try
        {
            var allParts = (await _partRepository.GetAllAsync()).ToList();
            
            if (!allParts.Any())
            {
                Console.WriteLine("Нет деталей для создания сборок.");
                return;
            }
            
            var assembly1 = new EngineAssembly("Турбина высокого давления", "Основной модуль турбины");
            var assembly2 = new EngineAssembly("Система сгорания", "Камера сгорания и сопутствующие компоненты");
            var assembly3 = new EngineAssembly("Полная сборка двигателя", "Все компоненты двигателя");
            
            assembly1.AddParts(allParts.Where(p => p.Material == MaterialType.Titanium 
                || p.Material == MaterialType.NickelAlloy).Take(3));
            
            assembly2.AddParts(allParts.Where(p => p.Material == MaterialType.Superalloy 
                || p.Material == MaterialType.Ceramic).Take(2));
            
            assembly3.AddParts(allParts);
            
            await AddAsync(assembly1);
            await AddAsync(assembly2);
            await AddAsync(assembly3);
            
            Console.WriteLine($"Создано {_assemblies.Count} тестовых сборок.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при инициализации сборок: {ex.Message}");
        }
    }
    
    public async Task<EngineAssembly?> GetByIdAsync(int id)
    {
        await Task.Delay(10);
        return _assemblies.FirstOrDefault(a => a.Id == id);
    }
    
    public async Task<IEnumerable<EngineAssembly>> GetAllAsync()
    {
        await Task.Delay(10);
        return _assemblies.AsReadOnly();
    }
    
    public async Task<IEnumerable<EngineAssembly>> FindAsync(System.Linq.Expressions.Expression<Func<EngineAssembly, bool>> predicate)
    {
        await Task.Delay(10);
        return _assemblies.AsQueryable().Where(predicate).ToList();
    }
    
    public async Task AddAsync(EngineAssembly assembly)
    {
        await Task.Delay(10);
        
        if (assembly == null)
            throw new ArgumentNullException(nameof(assembly));
        
        assembly.Id = _nextId++;
        assembly.CreatedAt = DateTime.UtcNow;
        _assemblies.Add(assembly);
        
        Console.WriteLine($"Добавлена сборка: {assembly.Name} (ID: {assembly.Id})");
    }
    
    public async Task UpdateAsync(EngineAssembly assembly)
    {
        await Task.Delay(10);
        
        var existing = await GetByIdAsync(assembly.Id);
        if (existing == null)
            throw new KeyNotFoundException($"Сборка с ID {assembly.Id} не найдена");
        
        assembly.UpdatedAt = DateTime.UtcNow;
        
        var index = _assemblies.FindIndex(a => a.Id == assembly.Id);
        if (index >= 0)
        {
            _assemblies[index] = assembly;
        }
        
        Console.WriteLine($"Обновлена сборка ID: {assembly.Id}");
    }
    
    public async Task RemoveAsync(EngineAssembly assembly)
    {
        await Task.Delay(10);
        _assemblies.Remove(assembly);
        Console.WriteLine($"Удалена сборка ID: {assembly.Id}");
    }
    
    public async Task<bool> ExistsAsync(int id)
    {
        var assembly = await GetByIdAsync(id);
        return assembly != null;
    }
    
    public async Task<int> CountAsync()
    {
        await Task.Delay(10);
        return _assemblies.Count;
    }
    
    // Реализация специфичных методов
    public async Task<IEnumerable<EngineAssembly>> GetByNameAsync(string name)
    {
        return await FindAsync(a => a.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
    }
    
    public async Task<IEnumerable<EngineAssembly>> GetHeavyAssembliesAsync(double minMass)
    {
        var assemblies = await GetAllAsync();
        var result = new List<EngineAssembly>();
        
        foreach (var assembly in assemblies)
        {
            var mass = await assembly.CalculateTotalMassAsync();
            if (mass >= minMass)
            {
                result.Add(assembly);
            }
        }
        
        return result.OrderByDescending(a => a.EstimatedTotalMass);
    }
    
    public async Task<IEnumerable<EngineAssembly>> GetByPartMaterialAsync(MaterialType material)
    {
        var assemblies = await GetAllAsync();
        return assemblies.Where(a => a.Parts.Any(p => p.Material == material));
    }
    
    public async Task AddPartToAssemblyAsync(int assemblyId, EnginePart part)
    {
        var assembly = await GetByIdAsync(assemblyId);
        if (assembly == null)
            throw new KeyNotFoundException($"Сборка с ID {assemblyId} не найдена");
        
        assembly.AddPart(part);
        await UpdateAsync(assembly);
        
        Console.WriteLine($"Деталь '{part.Name}' добавлена в сборку '{assembly.Name}'");
    }
    
    public async Task AddPartsToAssemblyAsync(int assemblyId, IEnumerable<EnginePart> parts)
    {
        var assembly = await GetByIdAsync(assemblyId);
        if (assembly == null)
            throw new KeyNotFoundException($"Сборка с ID {assemblyId} не найдена");
        
        assembly.AddParts(parts);
        await UpdateAsync(assembly);
        
        Console.WriteLine($"Добавлено {parts.Count()} деталей в сборку '{assembly.Name}'");
    }
    
    public async Task<bool> RemovePartFromAssemblyAsync(int assemblyId, int partId)
    {
        var assembly = await GetByIdAsync(assemblyId);
        if (assembly == null)
            throw new KeyNotFoundException($"Сборка с ID {assemblyId} не найдена");
        
        var removed = assembly.RemovePart(partId);
        if (removed)
        {
            await UpdateAsync(assembly);
            Console.WriteLine($"Деталь ID {partId} удалена из сборки '{assembly.Name}'");
        }
        
        return removed;
    }
    
    public async Task<IEnumerable<AssemblyStatistics>> GetAssemblyStatisticsAsync()
    {
        var assemblies = await GetAllAsync();
        
        return assemblies.Select(a => new AssemblyStatistics
        {
            AssemblyName = a.Name,
            PartCount = a.PartCount,
            TotalMass = a.EstimatedTotalMass,
            TotalCost = a.EstimatedTotalCost,
            AveragePartMass = a.PartCount > 0 ? a.EstimatedTotalMass / a.PartCount : 0,
            UniqueMaterialsCount = a.Parts.Select(p => p.Material).Distinct().Count()
        })
        .OrderByDescending(s => s.TotalMass);
    }
    
    public async Task<Dictionary<MaterialType, int>> GetMaterialUsageStatisticsAsync()
    {
        var assemblies = await GetAllAsync();
        var allParts = assemblies.SelectMany(a => a.Parts);
        
        return allParts
            .GroupBy(p => p.Material)
            .ToDictionary(
                g => g.Key,
                g => g.Count()
            );
    }
}