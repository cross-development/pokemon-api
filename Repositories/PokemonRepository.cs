using PokemonApi.Data;
using PokemonApi.Interfaces;
using PokemonApi.Models;

namespace PokemonApi.Repositories;

public class PokemonRepository : IPokemonRepository
{
    private readonly DataContext _context;

    public PokemonRepository(DataContext context)
    {
        _context = context;
    }

    public ICollection<Pokemon> GetPokemons()
    {
        return _context.Pokemon.OrderBy(pokemon => pokemon.Id).ToList();
    }

    public Pokemon GetPokemon(int pokeId)
    {
        return _context.Pokemon.FirstOrDefault(pokemon => pokemon.Id == pokeId);
    }

    public decimal GetPokemonRating(int pokeId)
    {
        IQueryable<Review> reviews = _context.Reviews.Where(review => review.Pokemon.Id == pokeId);

        if (!reviews.Any())
        {
            return 0;
        }

        return (decimal)reviews.Sum(review => review.Rating) / reviews.Count();
    }

    public bool PokemonExists(int pokeId)
    {
        return _context.Pokemon.Any(pokemon => pokemon.Id == pokeId);
    }
}