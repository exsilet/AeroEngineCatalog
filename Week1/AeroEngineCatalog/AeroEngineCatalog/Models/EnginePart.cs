namespace AeroEngineCatalog.Models;

public class EnginePart
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Material { get; set; } = string.Empty;
    public double Mass { get; set; }
    
    public override string ToString()
    {
        return $"{Id}: {Name} (Material: {Material}, Mass: {Mass} kg)";
    }
}