using System.Collections.Generic;
using System.Threading.Tasks;
using AeroEngineApi.Models;

namespace AeroEngineApi.Repositories;

public interface IPartRepository : IRepository<Part>
{
    Task<IEnumerable<Part>> GetByEngineIdAsync(int engineId);
    Task<IEnumerable<Part>> GetByMaterialAsync(string material);
}