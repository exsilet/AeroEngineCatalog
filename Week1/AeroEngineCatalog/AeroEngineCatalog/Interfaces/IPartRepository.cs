using AeroEngineCatalog.Models;

namespace AeroEngineCatalog.Interfaces;

public interface IPartRepository
{
    IEnumerable<EnginePart> GetAll();
    EnginePart? GetById(int id);
    void Add(EnginePart part);
    IEnumerable<EnginePart> GetByMaterial(string material);
}