using PokemonApi.Data;
using PokemonApi.Interfaces;
using PokemonApi.Models;

namespace PokemonApi.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly DataContext _context;

    public CategoryRepository(DataContext context)
    {
        _context = context;
    }

    public ICollection<Category> GetCategories()
    {
        return _context.Categories.ToList();
    }

    public Category GetCategory(int categoryId)
    {
        return _context.Categories.Where(category => category.Id == categoryId).FirstOrDefault();
    }

    public ICollection<Pokemon> GetPokemonByCategoryId(int categoryId)
    {
        return _context.PokemonCategories
            .Where(pokemon => pokemon.CategoryId == categoryId)
            .Select(category => category.Pokemon)
            .ToList();
    }

    public bool CategoriesExists(int categoryId)
    {
        return _context.Categories.Any(category => category.Id == categoryId);
    }

    public bool CreateCategory(Category category)
    {
        _context.Add(category);

        return Save();
    }

    public bool Save()
    {
        var saved = _context.SaveChanges();

        return saved > 0;
    }
}