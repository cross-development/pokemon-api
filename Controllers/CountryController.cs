using System.Net.Mime;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonApi.Dto;
using PokemonApi.Interfaces;
using PokemonApi.Models;

namespace PokemonApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CountryController : ControllerBase
{
    private readonly ICountryRepository _countryRepository;
    private readonly IMapper _mapper;

    public CountryController(ICountryRepository countryRepository, IMapper mapper)
    {
        _countryRepository = countryRepository;
        _mapper = mapper;
    }

    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(IEnumerable<CountryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult GetCountries()
    {
        var country = _mapper.Map<List<CountryDto>>(_countryRepository.GetCountries());

        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        return Ok(country);
    }

    [HttpGet("{countryId:int}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(CountryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetCountry(int countryId)
    {
        var isCountryExist = _countryRepository.CountryExists(countryId);

        if (!isCountryExist)
        {
            return NotFound();
        }

        var country = _mapper.Map<CountryDto>(_countryRepository.GetCountry(countryId));

        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        return Ok(country);
    }

    [HttpGet("owners/{ownerId:int}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(CountryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult GetCountryByOwnerId(int ownerId)
    {
        var country = _mapper.Map<CountryDto>(_countryRepository.GetCountryByOwnerId(ownerId));

        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        return Ok(country);
    }

    [HttpGet("{countryId:int}/owners")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(IEnumerable<OwnerDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult GetOwnersFromCountry(int countryId)
    {
        var owners = _mapper.Map<List<OwnerDto>>(_countryRepository.GetOwnersFromCountry(countryId));

        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        return Ok(owners);
    }

    [HttpPost]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult CreateCountry([FromBody] CountryDto countryDto)
    {
        if (countryDto == null)
        {
            return BadRequest(ModelState);
        }

        var country = _countryRepository.GetCountries()
            .Where(country => country.Name.Trim().ToUpper() == countryDto.Name.Trim().ToUpper())
            .FirstOrDefault();

        if (country != null)
        {
            ModelState.AddModelError("error", "Country already exists");

            return StatusCode(StatusCodes.Status409Conflict, ModelState);
        }

        var countryMap = _mapper.Map<Country>(countryDto);

        var isCountryCreated = _countryRepository.CreateCountry(countryMap);

        if (!isCountryCreated)
        {
            ModelState.AddModelError("error", "Something went wrong while saving");

            return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
        }

        return Ok("Successfully created");
    }
}
