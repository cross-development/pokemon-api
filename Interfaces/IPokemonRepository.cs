using PokemonApi.Models;

namespace PokemonApi.Interfaces;

public interface IPokemonRepository
{
    ICollection<Pokemon> GetPokemons();
    Pokemon GetPokemon(int pokeId);
    decimal GetPokemonRating(int pokeId);
    bool PokemonExists(int pokeId);
    bool CreatePokemon(int ownerId, int categoryId, Pokemon pokemon);
    bool Save();
}