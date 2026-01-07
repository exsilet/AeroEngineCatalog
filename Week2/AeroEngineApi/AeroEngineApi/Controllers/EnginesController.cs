using AeroEngineApi.Models;
using AeroEngineApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AeroEngineApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnginesController : ControllerBase
{
    private readonly IEngineRepository _repository;

    public EnginesController(IEngineRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Engine>>> GetEngines()
    {
        var engines = await _repository.GetAllWithPartsAsync();
        return Ok(engines);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Engine>> GetEngine(int id)
    {
        var engine = await _repository.GetWithPartsAsync(id);

        if (engine == null)
        {
            return NotFound();
        }

        return engine;
    }

    [HttpPost]
    public async Task<ActionResult<Engine>> CreateEngine(Engine engine)
    {
        var createdEngine = await _repository.AddAsync(engine);
        return CreatedAtAction(nameof(GetEngine), new { id = createdEngine.Id }, createdEngine);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEngine(int id, Engine engine)
    {
        if (id != engine.Id)
        {
            return BadRequest();
        }

        if (!await _repository.ExistsAsync(id))
        {
            return NotFound();
        }

        await _repository.UpdateAsync(engine);
        return NoContent();
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEngine(int id)
    {
        if (!await _repository.ExistsAsync(id))
        {
            return NotFound();
        }

        await _repository.DeleteAsync(id);
        return NoContent();
    }
    
    [HttpGet("{id}/total-mass")]
    public async Task<ActionResult<object>> GetEngineTotalMass(int id)
    {
        if (!await _repository.ExistsAsync(id))
        {
            return NotFound();
        }
        
        var totalMass = await _repository.CalculateTotalMassAsync(id);
        return Ok(new { EngineId = id, TotalMass = totalMass });
    }
}