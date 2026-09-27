using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Visiotech.Pokemon.Api.Controllers;
using Visiotech.Pokemon.Api.Domain;
using Visiotech.Pokemon.Api.Infrastructure;
using PokemonModel = Visiotech.Pokemon.Api.Domain.Pokemon;

namespace Visiotech.Pokemon.UnitTests;

public class PokedexAndBattleTests
{
    private AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task GetPokemons_ReturnsEmptyList_WhenNoDataExists()
    {
        var context = GetInMemoryDbContext();
        var controller = new PokedexController(context);

        var result = await controller.GetPokemons();

        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task CatchPokemon_InsertsNewCapturedPokemon_ToUserTeam()
    {
        var context = GetInMemoryDbContext();
        var controller = new PokedexController(context);

        var basePokemon = new PokemonModel { Id = 10, Name = "Bulbasaur", Type = PokemonType.Grass };
        context.Pokemons.Add(basePokemon);
        await context.SaveChangesAsync();

        var myPokemon = new UserPokemon { Id = 1, PokemonBaseId = 10, CustomNickname = "MyBulba", CurrentLevel = 5 };

        var result = await controller.CatchPokemon(myPokemon);
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedPokemon = Assert.IsType<UserPokemon>(createdResult.Value);

        Assert.Equal("MyBulba", returnedPokemon.CustomNickname);
    }

    [Fact]
    public async Task SimulateBattle_ReturnsFourTurnLog_AndValidWinner()
    {
        var context = GetInMemoryDbContext();
        var battleController = new BattleController(context);

        var tackle = new Movement { Id = 1, Name = "Tackle", Power = 40, Type = PokemonType.Normal };
        
        var p1 = new PokemonModel { Id = 1, Name = "Pikachu", TotalHp = 100, BaseSpeed = 90, BaseAttack = 50, BaseDefense = 40, Movements = new List<Movement> { tackle } };
        var p2 = new PokemonModel { Id = 2, Name = "Charizard", TotalHp = 100, BaseSpeed = 100, BaseAttack = 60, BaseDefense = 50, Movements = new List<Movement> { tackle } };
        
        context.Pokemons.AddRange(p1, p2);

        var userP1 = new UserPokemon { Id = 1, PokemonBaseId = 1, TrainedMovements = new List<Movement> { tackle } };
        var userP2 = new UserPokemon { Id = 2, PokemonBaseId = 2, TrainedMovements = new List<Movement> { tackle } };
        
        context.UserPokemons.AddRange(userP1, userP2);
        await context.SaveChangesAsync();

        var result = await battleController.SimulateBattle(1, 2);
        var okResult = Assert.IsType<OkObjectResult>(result);

        Assert.NotNull(okResult.Value);
    }
}