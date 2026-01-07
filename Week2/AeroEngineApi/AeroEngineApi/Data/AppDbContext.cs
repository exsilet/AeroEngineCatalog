using AeroEngineApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AeroEngineApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Part> Parts => Set<Part>();
    public DbSet<Engine> Engines => Set<Engine>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Engine>().HasMany(e => e.Parts).WithOne(e => e.Engine).HasForeignKey(e => e.EngineId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Part>().HasIndex(p=>p.Name);
        modelBuilder.Entity<Engine>().HasIndex(p=>p.Name).IsUnique();
    }
}