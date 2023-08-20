using AutoMapper;
using PokemonApi.Data;
using PokemonApi.Interfaces;
using PokemonApi.Models;

namespace PokemonApi.Repositories;

public class CountryRepository : ICountryRepository
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public CountryRepository(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public ICollection<Country> GetCountries()
    {
        return _context.Countries.ToList();
    }

    public Country GetCountry(int countryId)
    {
        return _context.Countries.Where(country => country.Id == countryId).FirstOrDefault();
    }

    public Country GetCountryByOwnerId(int ownerId)
    {
        return _context.Owners
            .Where(owner => owner.Id == ownerId)
            .Select(owner => owner.Country)
            .FirstOrDefault();
    }

    public ICollection<Owner> GetOwnersFromCountry(int countryId)
    {
        return _context.Owners.Where(owner => owner.Country.Id == countryId).ToList();
    }

    public bool CountryExists(int countryId)
    {
        return _context.Countries.Any(country => country.Id == countryId);
    }
}