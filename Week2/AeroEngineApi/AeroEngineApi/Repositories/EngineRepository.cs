using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AeroEngineApi.Data;
using AeroEngineApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AeroEngineApi.Repositories;

public class EngineRepository : IEngineRepository
{
    private readonly AppDbContext _context;

    public EngineRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Engine?> GetByIdAsync(int id)
    {
        return await _context.Engines.FindAsync(id);
    }

    public async Task<IEnumerable<Engine>> GetAllAsync()
    {
        return await _context.Engines.ToListAsync();
    }

    public async Task<Engine> AddAsync(Engine engine)
    {
        _context.Engines.Add(engine);
        await _context.SaveChangesAsync();
        return engine;
    }

    public async Task UpdateAsync(Engine engine)
    {
        _context.Entry(engine).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var engine = await GetByIdAsync(id);
        if (engine != null)
        {
            _context.Engines.Remove(engine);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Engines.AnyAsync(e => e.Id == id);
    }

    public async Task<Engine?> GetWithPartsAsync(int id)
    {
        return await _context.Engines.Include(e => e.Parts).FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<IEnumerable<Engine>> GetAllWithPartsAsync()
    {
        return await _context.Engines.Include(e => e.Parts).ToListAsync();
    }

    public async Task<double> CalculateTotalMassAsync(int engineId)
    {
        return await _context.Parts.Where(p => p.EngineId == engineId).SumAsync(p => p.Mass);
    }
}