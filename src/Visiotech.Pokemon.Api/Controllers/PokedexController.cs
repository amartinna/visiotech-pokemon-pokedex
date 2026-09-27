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
}