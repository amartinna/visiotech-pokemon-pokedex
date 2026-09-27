using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Visiotech.Pokemon.Api.Domain;
using Visiotech.Pokemon.Api.Infrastructure;
using PokemonModel = Visiotech.Pokemon.Api.Domain.Pokemon;

namespace Visiotech.Pokemon.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BattleController : ControllerBase
{
    private readonly AppDbContext _context;

    public BattleController(AppDbContext context)
    {
        _context = context;
    }

    // POST: api/battle/simulate
    // Receives the IDs of two captured Pokémon from your personal team and simulates the complete battle in phases
    [HttpPost("simulate")]
    public async Task<IActionResult> SimulateBattle([FromQuery] int playerPokemonId, [FromQuery] int rivalPokemonId)
    {
        var playerPokemon = await _context.UserPokemons
            .Include(u => u.PokemonBase)
            .Include(u => u.TrainedMovements)
            .FirstOrDefaultAsync(u => u.Id == playerPokemonId);

        var rivalPokemon = await _context.UserPokemons
            .Include(u => u.PokemonBase)
            .Include(u => u.TrainedMovements)
            .FirstOrDefaultAsync(u => u.Id == rivalPokemonId);

        // Validaciones defensivas iniciales
        if (playerPokemon == null || rivalPokemon == null || playerPokemon.PokemonBase == null || rivalPokemon.PokemonBase == null)
        {
            return NotFound("One or both of the selected customized Pokemons do not exist in your team.");
        }

        if (!playerPokemon.TrainedMovements.Any() || !rivalPokemon.TrainedMovements.Any())
        {
            return BadRequest("Both combatants must have at least one trained movement equipped to fight.");
        }

        // Hacemos copia de los estados de salud de los Pokémon base para la simulación
        int playerHp = playerPokemon.PokemonBase.TotalHp;
        int rivalHp = rivalPokemon.PokemonBase.TotalHp;

        var battleLog = new List<string>
        {
            $"--- BATTLE ARENA INITIALIZED ---",
            $"Player: {playerPokemon.PokemonBase.Name} (HP: {playerHp}) vs Rival: {rivalPokemon.PokemonBase.Name} (HP: {rivalHp})"
        };

        int round = 1;

        // Bucle de simulación por turnos hasta que la salud de uno caiga a cero
        while (playerHp > 0 && rivalHp > 0)
        {
            battleLog.Add($"\n[Round {round}]");

            // Algoritmo senior: Determinamos el orden del turno basándonos en la velocidad base (BaseSpeed) de cada especie
            bool playerGoesFirst = playerPokemon.PokemonBase.BaseSpeed >= rivalPokemon.PokemonBase.BaseSpeed;

            if (playerGoesFirst)
            {
                // Turno del Jugador
                rivalHp = ExecuteTurn(playerPokemon.PokemonBase, playerPokemon.TrainedMovements.First(), rivalPokemon.PokemonBase, rivalHp, "Player", battleLog);
                if (rivalHp <= 0) break;

                // Turno del Rival
                playerHp = ExecuteTurn(rivalPokemon.PokemonBase, rivalPokemon.TrainedMovements.First(), playerPokemon.PokemonBase, playerHp, "Rival", battleLog);
            }
            else
            {
                // Turno del Rival primero
                playerHp = ExecuteTurn(rivalPokemon.PokemonBase, rivalPokemon.TrainedMovements.First(), playerPokemon.PokemonBase, playerHp, "Rival", battleLog);
                if (playerHp <= 0) break;

                // Turno del Jugador
                rivalHp = ExecuteTurn(playerPokemon.PokemonBase, playerPokemon.TrainedMovements.First(), rivalPokemon.PokemonBase, rivalHp, "Player", battleLog);
            }

            round++;
            
            // break si la batalla se prolonga demasiado (más de 50 rondas)
            if (round > 50)
            {
                battleLog.Add("The battle has drawn due to extreme endurance limit.");
                break;
            }
        }

        // winner determination based on remaining HP
        battleLog.Add("\n--- BATTLE FINISHED ---");
        battleLog.Add(playerHp > 0 ? $"Victory for Player's {playerPokemon.PokemonBase.Name}!" : $"Victory for Rival's {rivalPokemon.PokemonBase.Name}!");

        return Ok(new
        {
            Status = "Completed",
            TotalRounds = round,
            Winner = playerHp > 0 ? playerPokemon.PokemonBase.Name : rivalPokemon.PokemonBase.Name,
            Log = battleLog
        });
    }

    private int ExecuteTurn(PokemonModel attacker, Movement move, PokemonModel defender, int defenderCurrentHp, string attackerName, List<string> log)
    {
        int damage = BattleEngine.CalculateDamage(attacker, move, defender);
        int newHp = Math.Max(0, defenderCurrentHp - damage);

        double effectiveness = BattleEngine.GetEffectiveness(move.Type, defender.Type);
        string effectivenessComment = effectiveness > 1.0 ? "It's super effective!" : effectiveness == 0.0 ? "It had no effect..." : "";

        log.Add($"{attackerName}'s {attacker.Name} uses {move.Name} dealing {damage} HP damage. {effectivenessComment} (Target HP left: {newHp})");
        
        return newHp;
    }
}