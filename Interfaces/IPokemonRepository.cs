using PokemonApi.Models;

namespace PokemonApi.Interfaces;

public interface IPokemonRepository
{
    ICollection<Pokemon> GetPokemons();
}