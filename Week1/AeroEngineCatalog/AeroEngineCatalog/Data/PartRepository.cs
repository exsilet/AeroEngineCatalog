using AeroEngineCatalog.Interfaces;
using AeroEngineCatalog.Models;

namespace AeroEngineCatalog.Data;

public class PartRepository : IPartRepository
{
    private readonly List<EnginePart> _parts;
    
    public PartRepository()
    {
        _parts = new List<EnginePart>
        {
            new() { Id = 1, Name = "Turbine Blade", Material = "Titanium", Mass = 2.5 },
            new() { Id = 2, Name = "Compressor Disk", Material = "Nickel Alloy", Mass = 15.8 },
            new() { Id = 3, Name = "Combustion Chamber", Material = "Superalloy", Mass = 32.1 },
            new() { Id = 4, Name = "Fan Blade", Material = "Titanium", Mass = 3.2 },
            new() { Id = 5, Name = "Nozzle Guide Vane", Material = "Ceramic", Mass = 1.8 },
            new() { Id = 6, Name = "Shaft", Material = "Steel", Mass = 45.7 },
            new() { Id = 7, Name = "Bearing Housing", Material = "Aluminum", Mass = 12.3 },
            new() { Id = 8, Name = "Fuel Nozzle", Material = "Stainless Steel", Mass = 0.9 }
        };
    }
    
    public IEnumerable<EnginePart> GetAll() => _parts;
    
    public EnginePart? GetById(int id) => _parts.FirstOrDefault(p => p.Id == id);
    
    public void Add(EnginePart part)
    {
        part.Id = _parts.Count > 0 ? _parts.Max(p => p.Id) + 1 : 1;
        _parts.Add(part);
    }
    
    public IEnumerable<EnginePart> GetByMaterial(string material)
    {
        return _parts
            .Where(p => p.Material.Equals(material, StringComparison.OrdinalIgnoreCase))
            .OrderBy(p => p.Name);
    }
}