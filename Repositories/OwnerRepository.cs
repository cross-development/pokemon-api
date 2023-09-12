﻿using PokemonApi.Data;
using PokemonApi.Interfaces;
using PokemonApi.Models;

namespace PokemonApi.Repositories;

public class OwnerRepository : IOwnerRepository
{
    private readonly DataContext _context;

    public OwnerRepository(DataContext context)
    {
        _context = context;
    }

    public ICollection<Owner> GetOwners()
    {
        return _context.Owners.ToList();
    }

    public Owner GetOwner(int ownerId)
    {
        return _context.Owners.Where(owner => owner.Id == ownerId).FirstOrDefault();
    }

    public ICollection<Owner> GetOwnerOfAPokemon(int pokeId)
    {
        return _context.PokemonOwners
            .Where(p => p.Pokemon.Id == pokeId)
            .Select(o => o.Owner)
            .ToList();
    }

    public ICollection<Pokemon> GetPokemonByOwner(int ownerId)
    {
        return _context.PokemonOwners
            .Where(p => p.Owner.Id == ownerId)
            .Select(p => p.Pokemon)
            .ToList();
    }

    public bool OwnerExists(int ownerId)
    {
        return _context.Owners.Any(owner => owner.Id == ownerId);
    }

    public bool CreateOwner(Owner owner)
    {
        _context.Add(owner);

        return Save();
    }

    public bool UpdateOwner(Owner owner)
    {
        _context.Update(owner);

        return Save();
    }

    public bool Save()
    {
        var saved = _context.SaveChanges();

        return saved > 0;
    }
}