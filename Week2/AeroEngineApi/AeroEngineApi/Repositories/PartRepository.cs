using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AeroEngineApi.Data;
using AeroEngineApi.Models;

namespace AeroEngineApi.Repositories;

public class PartRepository : IPartRepository
{
    private readonly AppDbContext _context;

    public PartRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Part?> GetByIdAsync(int id)
    {
        return await _context.Parts.FindAsync(id);
    }

    public async Task<IEnumerable<Part>> GetAllAsync()
    {
        return await _context.Parts.ToListAsync();
    }

    public async Task<Part> AddAsync(Part part)
    {
        _context.Parts.Add(part);
        await _context.SaveChangesAsync();
        return part;
    }

    public async Task UpdateAsync(Part part)
    {
        _context.Entry(part).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var part = await GetByIdAsync(id);
        if (part != null)
        {
            _context.Parts.Remove(part);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Parts.AnyAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Part>> GetByEngineIdAsync(int engineId)
    {
        return await _context.Parts
            .Where(p => p.EngineId == engineId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Part>> GetByMaterialAsync(string material)
    {
        return await _context.Parts
            .Where(p => p.Material == material)
            .ToListAsync();
    }
}