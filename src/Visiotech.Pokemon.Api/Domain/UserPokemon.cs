namespace Visiotech.Pokemon.Api.Domain;

public class UserPokemon
{
    public int Id { get; set; }
    public string CustomNickname { get; set; } = string.Empty; // Optional nickname
    public int PokemonBaseId { get; set; } // Link to the base template
    public Pokemon? PokemonBase { get; set; }
    public int CurrentLevel { get; set; }
    public List<Movement> TrainedMovements { get; set; } = new(); // Their 4 trained moves
}