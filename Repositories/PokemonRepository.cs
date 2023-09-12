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

    public bool CreatePokemon(int ownerId, int categoryId, Pokemon pokemon)
    {
        var pokemonOwnerEntity = _context.Owners.Where(owner => owner.Id == ownerId).FirstOrDefault();
        var pokemonCategoryEntity = _context.Categories.Where(category => category.Id == categoryId).FirstOrDefault();

        var pokemonOwner = new PokemonOwner
        {
            Owner = pokemonOwnerEntity,
            Pokemon = pokemon,
        };

        _context.Add(pokemonOwner);

        var pokemonCategory = new PokemonCategory
        {
            Category = pokemonCategoryEntity,
            Pokemon = pokemon,
        };

        _context.Add(pokemonCategory);
        _context.Add(pokemon);

        return Save();
    }

    public bool UpdatePokemon(int ownerId, int categoryId, Pokemon pokemon)
    {
        _context.Update(pokemon);

        return Save();
    }

    public bool Save()
    {
        var saved = _context.SaveChanges();

        return saved > 0;
    }
}