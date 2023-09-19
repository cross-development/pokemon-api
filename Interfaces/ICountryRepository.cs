using PokemonApi.Models;

namespace PokemonApi.Interfaces;

public interface ICountryRepository
{
    ICollection<Country> GetCountries();
    Country GetCountry(int countryId);
    Country GetCountryByOwnerId(int ownerId);
    ICollection<Owner> GetOwnersFromCountry(int countryId);
    bool CountryExists(int countryId);
    bool CreateCountry(Country country);
    bool UpdateCountry(Country country);
    bool DeleteCountry(Country country);
    bool Save();
}