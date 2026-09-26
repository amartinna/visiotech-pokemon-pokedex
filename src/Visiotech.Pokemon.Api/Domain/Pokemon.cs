namespace Visiotech.Pokemon.Api.Domain;

public class Pokemon
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public PokemonType Type { get; set; }
    public int Level { get; set; }
    public int CurrentHp { get; set; }
    public int TotalHp { get; set; }
    public int BaseAttack { get; set; }
    public int BaseDefense { get; set; }
    public int BaseSpecialAttack { get; set; }
    public int BaseSpecialDefense { get; set; }
    public int BaseSpeed { get; set; }
    public List<Movement> Movements { get; set; } = new();
}