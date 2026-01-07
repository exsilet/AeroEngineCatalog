using System;
using System.Collections.Generic;

namespace AeroEngineApi.Models;

public class Engine
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public double Thrust { get; set; }
    public double BypassRatio { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    
    public List<Part> Parts { get; set; } = new();
}