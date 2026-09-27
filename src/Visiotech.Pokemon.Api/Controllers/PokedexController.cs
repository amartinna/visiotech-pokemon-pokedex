using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    // Returns a single base Pokémon by its unique identifier
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
    // Inserts a new base Pokémon record into the Pokedex
    [HttpPost("pokemon")]
    public async Task<ActionResult<PokemonModel>> CreatePokemon(PokemonModel pokemon)
    {
        _context.Pokemons.Add(pokemon);
        await _context.SaveChangesAsync();
        
        return CreatedAtAction(nameof(GetPokemonById), new { id = pokemon.Id }, pokemon);
    }
}