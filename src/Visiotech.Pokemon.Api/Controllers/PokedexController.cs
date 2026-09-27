using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Visiotech.Pokemon.Api.Domain;
using Visiotech.Pokemon.Api.Infrastructure;
using PokemonModel = Visiotech.Pokemon.Api.Domain.Pokemon;

namespace Visiotech.Pokemon.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PokedexController : ControllerBase
{
    private readonly AppDbContext _context;

    public PokedexController(AppDbContext context)
    {
        _context = context;
    }

    // 1. POKEMONS BASE (FULL CRUD)
    // =========================================================================

    // GET: api/pokedex/pokemon
    // Returns the clean list without loading heavy collections in a loop
    [HttpGet("pokemon")]
    public async Task<ActionResult<IEnumerable<PokemonModel>>> GetPokemons()
    {
        return await _context.Pokemons.ToListAsync();
    }

    // GET: api/pokedex/pokemon/{id}
    // Returns a single base Pokemon by its unique identifier
    [HttpGet("pokemon/{id}")]
    public async Task<ActionResult<PokemonModel>> GetPokemonById(int id)
    {
        var pokemon = await _context.Pokemons.FindAsync(id);

        if (pokemon == null)
        {
            return NotFound($"Pokemon with ID {id} not found in the database.");
        }

        return Ok(pokemon);
    }

    // POST: api/pokedex/pokemon
    // Inserts a new base Pokemon record into the Pokedex
    [HttpPost("pokemon")]
    public async Task<ActionResult<PokemonModel>> CreatePokemon(PokemonModel pokemon)
    {
        _context.Pokemons.Add(pokemon);
        await _context.SaveChangesAsync();
        
        return CreatedAtAction(nameof(GetPokemonById), new { id = pokemon.Id }, pokemon);
    }

    // PUT: api/pokedex/pokemon/{id}
    // Updates the statistics or data of an existing Pokemon
    [HttpPut("pokemon/{id}")]
    public async Task<IActionResult> UpdatePokemon(int id, PokemonModel pokemon)
    {
        if (id != pokemon.Id)
        {
            return BadRequest("The URL ID parameter does not match the body entity ID.");
        }

        _context.Entry(pokemon).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Pokemons.AnyAsync(p => p.Id == id))
            {
                return NotFound($"Pokemon with ID {id} no longer exists.");
            }
            throw;
        }

        return NoContent();
    }

    // DELETE: api/pokedex/pokemon/{id}
    // Deletes a Pokemon from the Pokedex by its unique identifier
    [HttpDelete("pokemon/{id}")]
    public async Task<IActionResult> DeletePokemon(int id)
    {
        var pokemon = await _context.Pokemons.FindAsync(id);
        if (pokemon == null)
        {
            return NotFound($"Pokemon with ID {id} not found.");
        }

        _context.Pokemons.Remove(pokemon);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // =========================================================================
    // 2. MOVEMENTS (FULL CRUD)
    // =========================================================================

    // GET: api/pokedex/movements
    // Returns the complete dictionary of available moves in the database
    [HttpGet("movements")]
    public async Task<ActionResult<IEnumerable<Movement>>> GetMovements()
    {
        return await _context.Movements.ToListAsync();
    }

    // GET: api/pokedex/movements/{id}
    // Returns a single movement by its unique identifier
    [HttpGet("movements/{id}")]
    public async Task<ActionResult<Movement>> GetMovementById(int id)
    {
        var movement = await _context.Movements.FindAsync(id);

        if (movement == null)
        {
            return NotFound($"Movement with ID {id} not found.");
        }

        return Ok(movement);
    }

    // POST: api/pokedex/movements
    // Creates a new movement in the system (includes its elemental type)
    [HttpPost("movements")]
    public async Task<ActionResult<Movement>> CreateMovement(Movement movement)
    {
        _context.Movements.Add(movement);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMovementById), new { id = movement.Id }, movement);
    }

    // PUT: api/pokedex/movements/{id}
    // Updates the parameters or power of an existing movement
    [HttpPut("movements/{id}")]
    public async Task<IActionResult> UpdateMovement(int id, Movement movement)
    {
        if (id != movement.Id)
        {
            return BadRequest("The parameter ID does not match the entity ID.");
        }

        _context.Entry(movement).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Movements.AnyAsync(m => m.Id == id))
            {
                return NotFound($"Movement with ID {id} does not exist.");
            }
            throw;
        }

        return NoContent();
    }

    // DELETE: api/pokedex/movements/{id}
    // Deletes a movement from the global dictionary by its unique identifier
    [HttpDelete("movements/{id}")]
    public async Task<IActionResult> DeleteMovement(int id)
    {
        var movement = await _context.Movements.FindAsync(id);
        if (movement == null)
        {
            return NotFound($"Movement with ID {id} not found.");
        }

        _context.Movements.Remove(movement);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // =========================================================================
    // 3. USERPOKEMONS WITH THEIR 4 MOVEMENTS (FULL CRUD)
    // =========================================================================

    // GET: api/pokedex/my-pokemon
    // Returns the list of your personal team or collection of captured Pokemons with their movements
    [HttpGet("my-pokemon")]
    public async Task<ActionResult<IEnumerable<UserPokemon>>> GetMyPokemons()
    {
        return await _context.UserPokemons
            .Include(u => u.PokemonBase)
            .Include(u => u.TrainedMovements)
            .ToListAsync();
    }

    // GET: api/pokedex/my-pokemon/{id}
    // Returns a single Pokemon from your personal team by its unique identifier, including its trained movements
    [HttpGet("my-pokemon/{id}")]
    public async Task<ActionResult<UserPokemon>> GetMyPokemonById(int id)
    {
        var myPokemon = await _context.UserPokemons
            .Include(u => u.PokemonBase)
            .Include(u => u.TrainedMovements)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (myPokemon == null)
        {
            return NotFound($"Captured Pokemon with ID {id} not found in your team.");
        }

        return Ok(myPokemon);
    }

    // POST: api/pokedex/my-pokemon
    // Captures or adds a Pokemon to your personal team by linking it to a base species
    [HttpPost("my-pokemon")]
    public async Task<ActionResult<UserPokemon>> CatchPokemon(UserPokemon myPokemon)
    {
        var baseSpecie = await _context.Pokemons.FindAsync(myPokemon.PokemonBaseId);
        if (baseSpecie == null)
        {
            return BadRequest("The referenced PokemonBaseId does not exist.");
        }

        if (myPokemon.TrainedMovements.Count > 4)
        {
            return BadRequest("A captured Pokemon cannot start with more than 4 movements.");
        }

        _context.UserPokemons.Add(myPokemon);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMyPokemonById), new { id = myPokemon.Id }, myPokemon);
    }

    // PUT: api/pokedex/my-pokemon/{id}/teach-move/{movementId}
    // Teaches or modifies the trained movements of the Pokemon (Ensures the 4-move rule)
    [HttpPut("my-pokemon/{id}/teach-move/{movementId}")]
    public async Task<IActionResult> TeachMovement(int id, int movementId)
    {
        var myPokemon = await _context.UserPokemons
            .Include(u => u.TrainedMovements)
            .FirstOrDefaultAsync(u => u.Id == id);

        var movement = await _context.Movements.FindAsync(movementId);

        if (myPokemon == null || movement == null)
        {
            return NotFound("Captured Pokemon or Movement not found in database.");
        }

        if (myPokemon.TrainedMovements.Count >= 4)
        {
            return BadRequest("Action denied. This Pokemon already has the maximum limit of 4 movements equipped.");
        }

        if (myPokemon.TrainedMovements.Any(m => m.Id == movementId))
        {
            return BadRequest("This Pokemon already knows the requested movement.");
        }

        myPokemon.TrainedMovements.Add(movement);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/pokedex/my-pokemon/{id}
    // Releases or removes a Pokemon from your personal team
    [HttpDelete("my-pokemon/{id}")]
    public async Task<IActionResult> ReleasePokemon(int id)
    {
        var myPokemon = await _context.UserPokemons.FindAsync(id);
        if (myPokemon == null)
        {
            return NotFound($"Captured Pokemon with ID {id} not found in your team.");
        }

        _context.UserPokemons.Remove(myPokemon);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // =========================================================================
    // INTERMEDIATE CROSS-QUERY (Relación: movimientos -> tipo -> Pokemon)
    // =========================================================================

    // GET: api/pokedex/pokemon/{id}/movements
    // Returns the list of movements assigned to a specific Pokemon by its unique identifier
    [HttpGet("pokemon/{id}/movements")]
    public async Task<ActionResult<IEnumerable<Movement>>> GetPokemonMovements(int id)
    {
        var pokemon = await _context.Pokemons
            .Include(p => p.Movements)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pokemon == null)
        {
            return NotFound($"Pokemon with ID {id} not found.");
        }

        // Assigned movements
        return Ok(pokemon.Movements);
    }
}