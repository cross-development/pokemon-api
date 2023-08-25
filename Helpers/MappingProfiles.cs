using AutoMapper;
using PokemonApi.Dto;
using PokemonApi.Models;

namespace PokemonApi.Helpers;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<Pokemon, PokemonDto>();
        CreateMap<Category, CategoryDto>();
        CreateMap<Country, CountryDto>();
        CreateMap<Owner, OwnerDto>();
        CreateMap<Review, ReviewDto>();
        CreateMap<Reviewer, ReviewerDto>();
    }
}
