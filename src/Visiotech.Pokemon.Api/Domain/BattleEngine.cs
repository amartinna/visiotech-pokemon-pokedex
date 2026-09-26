namespace Visiotech.Pokemon.Api.Domain;

public static class BattleEngine
{
    private static readonly Dictionary<(PokemonType Attacker, PokemonType Defender), double> TypeEffectiveness = new()
    {
        { (PokemonType.Fire, PokemonType.Grass), 2.0 },
        { (PokemonType.Fire, PokemonType.Water), 0.5 },
        { (PokemonType.Fire, PokemonType.Fire), 0.5 },
        { (PokemonType.Water, PokemonType.Fire), 2.0 },
        { (PokemonType.Water, PokemonType.Grass), 0.5 },
        { (PokemonType.Water, PokemonType.Ground), 2.0 },
        { (PokemonType.Grass, PokemonType.Water), 2.0 },
        { (PokemonType.Grass, PokemonType.Fire), 0.5 },
        { (PokemonType.Grass, PokemonType.Ground), 2.0 },
        { (PokemonType.Electric, PokemonType.Water), 2.0 },
        { (PokemonType.Electric, PokemonType.Ground), 0.0 }, // No damage
        { (PokemonType.Ground, PokemonType.Flying), 0.0 }
    };

    public static int CalculateDamage(Pokemon attacker, Movement movement, Pokemon defender)
    {
        if (!attacker.Movements.Any(m => m.Id == movement.Id || m.Name == movement.Name))
        {
            throw new ArgumentException($"Pokemon {attacker.Name} does not know the move {movement.Name}.");
        }

        throw new NotImplementedException("Damage calculation logic is not implemented yet.");
    }
}