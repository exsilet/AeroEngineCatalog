using System;

namespace AeroEngineApi.Models;

public class Part
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Material { get; set; } = string.Empty;
    public double Mass { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    
    public int? EngineId { get; set; }
    public Engine? Engine { get; set; }
}