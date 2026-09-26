namespace Visiotech.Pokemon.Api.Domain;

public class Movement
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Power { get; set; }
    public PokemonType Type { get; set; }
} 