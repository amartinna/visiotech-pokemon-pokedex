using Microsoft.EntityFrameworkCore;
using PokemonModel = Visiotech.Pokemon.Api.Domain.Pokemon;
using Visiotech.Pokemon.Api.Domain;

namespace Visiotech.Pokemon.Api.Infrastructure;

public class AppDbContext : DbContext
{
    public DbSet<PokemonModel> Pokemons => Set<PokemonModel>();
    public DbSet<Movement> Movements => Set<Movement>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PokemonModel>()
            .HasMany(p => p.Movements)
            .WithMany();

        var thunderbolt = new Movement { Id = 1, Name = "Thunderbolt", Power = 90, Type = PokemonType.Electric };
        var flamethrower = new Movement { Id = 2, Name = "Flamethrower", Power = 90, Type = PokemonType.Fire };
        var wingAttack = new Movement { Id = 3, Name = "Wing Attack", Power = 60, Type = PokemonType.Flying };

        modelBuilder.Entity<Movement>().HasData(thunderbolt, flamethrower, wingAttack);

        modelBuilder.Entity<PokemonModel>().HasData(
            new PokemonModel 
            { 
                Id = 1, Name = "Pikachu", Type = PokemonType.Electric, Level = 50, 
                CurrentHp = 110, TotalHp = 110, BaseAttack = 55, BaseDefense = 40, 
                BaseSpecialAttack = 50, BaseSpecialDefense = 50, BaseSpeed = 90 
            },
            new PokemonModel 
            { 
                Id = 2, Name = "Charizard", Type = PokemonType.Fire, Level = 50, 
                CurrentHp = 153, TotalHp = 153, BaseAttack = 84, BaseDefense = 78, 
                BaseSpecialAttack = 109, BaseSpecialDefense = 85, BaseSpeed = 100 
            }
        );
    }
}