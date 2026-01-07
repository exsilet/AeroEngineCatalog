using System.Collections.Generic;
using System.Threading.Tasks;
using AeroEngineApi.Models;

namespace AeroEngineApi.Repositories;

public interface IEngineRepository : IRepository<Engine>
{
    Task<Engine?> GetWithPartsAsync(int id);
    Task<IEnumerable<Engine>> GetAllWithPartsAsync();
    Task<double> CalculateTotalMassAsync(int engineId);
}