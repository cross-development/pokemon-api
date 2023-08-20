﻿using PokemonApi.Models;

namespace PokemonApi.Interfaces;

public interface ICategoryRepository
{
    ICollection<Category> GetCategories();
    Category GetCategory(int categoryId);
    ICollection<Pokemon> GetPokemonByCategoryId(int categoryId);
    bool CategoriesExists(int categoryId);
}