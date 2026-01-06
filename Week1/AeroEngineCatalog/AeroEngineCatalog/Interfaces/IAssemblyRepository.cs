using AeroEngineCatalog.Models;

namespace AeroEngineCatalog.Interfaces;

public interface IAssemblyRepository : IRepository<EngineAssembly>
{
    Task<IEnumerable<EngineAssembly>> GetByNameAsync(string name);
    Task<IEnumerable<EngineAssembly>> GetHeavyAssembliesAsync(double minMass);
    Task<IEnumerable<EngineAssembly>> GetByPartMaterialAsync(MaterialType material);
    
    // Методы для работы с деталями в сборках
    Task AddPartToAssemblyAsync(int assemblyId, EnginePart part);
    Task AddPartsToAssemblyAsync(int assemblyId, IEnumerable<EnginePart> parts);
    Task<bool> RemovePartFromAssemblyAsync(int assemblyId, int partId);
    
    // Статистика
    Task<IEnumerable<AssemblyStatistics>> GetAssemblyStatisticsAsync();
    Task<Dictionary<MaterialType, int>> GetMaterialUsageStatisticsAsync();
}

public class AssemblyStatistics
{
    public string AssemblyName { get; set; } = string.Empty;
    public int PartCount { get; set; }
    public double TotalMass { get; set; }
    public decimal TotalCost { get; set; }
    public double AveragePartMass { get; set; }
    public int UniqueMaterialsCount { get; set; }
    
    public override string ToString()
    {
        return $"{AssemblyName}: {PartCount} деталей, {TotalMass:F2} кг, ${TotalCost:F2}";
    }
}