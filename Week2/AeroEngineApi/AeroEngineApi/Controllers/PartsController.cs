using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AeroEngineApi.Models;
using AeroEngineApi.Repositories;

namespace AeroEngineApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PartsController : ControllerBase
{
    private readonly IPartRepository _partRepository;
    private readonly IEngineRepository _engineRepository;

    public PartsController(IPartRepository partRepository, IEngineRepository engineRepository)
    {
        _partRepository = partRepository;
        _engineRepository = engineRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Part>>> GetParts()
    {
        var parts = await _partRepository.GetAllAsync();
        return Ok(parts);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Part>> GetPart(int id)
    {
        var part = await _partRepository.GetByIdAsync(id);

        if (part == null)
        {
            return NotFound();
        }

        return part;
    }

    [HttpPost]
    public async Task<ActionResult<Part>> PostPart(Part part)
    {
        if (part.EngineId.HasValue && !await _engineRepository.ExistsAsync(part.EngineId.Value))
        {
            return BadRequest($"Двигатель с ID {part.EngineId.Value} не существует");
        }

        var createdPart = await _partRepository.AddAsync(part);
        return CreatedAtAction(nameof(GetPart), new { id = createdPart.Id }, createdPart);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePart(int id, Part part)
    {
        if (id != part.Id)
        {
            return BadRequest();
        }

        if (!await _partRepository.ExistsAsync(id))
        {
            return NotFound();
        }
        
        if (part.EngineId.HasValue && !await _engineRepository.ExistsAsync(part.EngineId.Value))
        {
            return BadRequest($"Двигатель с ID {part.EngineId.Value} не существует");
        }

        await _partRepository.UpdateAsync(part);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePart(int id)
    {
        if (!await _partRepository.ExistsAsync(id))
        {
            return NotFound();
        }

        await _partRepository.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("by-engine/{engineId}")]
    public async Task<ActionResult<IEnumerable<Part>>> GetPartsByEngine(int engineId)
    {
        if (!await _engineRepository.ExistsAsync(engineId))
        {
            return NotFound($"Двигатель с ID {engineId} не существует");
        }

        var parts = await _partRepository.GetByEngineIdAsync(engineId);
        return Ok(parts);
    }

    [HttpGet("by-material/{material}")]
    public async Task<ActionResult<IEnumerable<Part>>> GetPartsByMaterial(string material)
    {
        var parts = await _partRepository.GetByMaterialAsync(material);
        return Ok(parts);
    }
}