using System.Linq.Expressions;
using AeroEngineCatalog.Models;

namespace AeroEngineCatalog.Interfaces;

public interface IPartRepository: IRepository<EnginePart>
{
    Task<IEnumerable<EnginePart>> GetByMaterialAsync(MaterialType material);
    Task<IEnumerable<EnginePart>> GetHeavierThanAsync(double minMass);
    Task<IEnumerable<EnginePart>> GetLighterThanAsync(double maxMass);
    Task<double> GetTotalMassAsync();
    Task<double> CalculateTotalMassAsync(IEnumerable<int> partIds);
}