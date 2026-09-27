using Visiotech.Pokemon.Api.Domain;
using PokemonModel = Visiotech.Pokemon.Api.Domain.Pokemon;

namespace Visiotech.Pokemon.UnitTests;

public class BattleEngineTests
{
    [Theory]
    [InlineData(PokemonType.Fire, PokemonType.Grass, 2.0)]
    [InlineData(PokemonType.Fire, PokemonType.Water, 0.5)]
    [InlineData(PokemonType.Electric, PokemonType.Ground, 0.0)]
    [InlineData(PokemonType.Normal, PokemonType.Normal, 1.0)]
    public void GetEffectiveness_ReturnsCorrectMultiplier_BasedOnVisiotechMatrix(PokemonType attacker, PokemonType defender, double expected)
    {
        var result = BattleEngine.GetEffectiveness(attacker, defender);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void CalculateDamage_ThrowsArgumentException_WhenPokemonDoesNotKnowTheMovement()
    {
        var attacker = new PokemonModel { Name = "Pikachu", Level = 50, BaseAttack = 55 };
        var defender = new PokemonModel { Name = "Charmander", BaseDefense = 43 };
        var unknownMove = new Movement { Id = 99, Name = "Hydro Pump", Type = PokemonType.Water, Power = 110 };

        // Al no estar el movimiento en la lista del atacante, debe disparar la excepción defensiva
        Assert.Throws<ArgumentException>(() => BattleEngine.CalculateDamage(attacker, unknownMove, defender));
    }

    [Fact]
    public void CalculateDamage_ReturnsValidInteger_ApplyingMathematicalFormula()
    {
        var thunderbolt = new Movement { Id = 1, Name = "Thunderbolt", Power = 90, Type = PokemonType.Electric };
        
        var attacker = new PokemonModel 
        { 
            Name = "Pikachu", Level = 50, BaseAttack = 55, 
            Movements = new List<Movement> { thunderbolt } 
        };
        
        var defender = new PokemonModel { Name = "Squirtle", BaseDefense = 65, Type = PokemonType.Water };

        var damage = BattleEngine.CalculateDamage(attacker, thunderbolt, defender);

        // El daño calculado según la fórmula debe ser mayor que cero
        Assert.True(damage > 0);
    }
}