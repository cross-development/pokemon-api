using System.Net.Mime;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonApi.Dto;
using PokemonApi.Interfaces;
using PokemonApi.Models;

namespace PokemonApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PokemonController : ControllerBase
{
    private readonly IPokemonRepository _pokemonRepository;
    private readonly IMapper _mapper;

    public PokemonController(IPokemonRepository pokemonRepository, IMapper mapper)
    {
        _pokemonRepository = pokemonRepository;
        _mapper = mapper;
    }

    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(IEnumerable<PokemonDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult GetPokemons()
    {
        var pokemons = _mapper.Map<List<PokemonDto>>(_pokemonRepository.GetPokemons());

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok(pokemons);
    }

    [HttpGet("{pokeId:int}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(PokemonDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetPokemon(int pokeId)
    {
        var isPokemonExist = _pokemonRepository.PokemonExists(pokeId);

        if (!isPokemonExist)
        {
            return NotFound();
        }

        var pokemon = _mapper.Map<PokemonDto>(_pokemonRepository.GetPokemon(pokeId));

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok(pokemon);
    }

    [HttpGet("{pokeId:int}/rating")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(decimal), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetPokemonRating(int pokeId)
    {
        var isPokemonExist = _pokemonRepository.PokemonExists(pokeId);

        if (!isPokemonExist)
        {
            return NotFound();
        }

        var rating = _pokemonRepository.GetPokemonRating(pokeId);

        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        return Ok(rating);
    }

    [HttpPost]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult CreatePokemon([FromQuery] int ownerId, [FromQuery] int categoryId, [FromBody] PokemonDto pokemonDto)
    {
        if (pokemonDto == null)
        {
            return BadRequest(ModelState);
        }

        var pokemon = _pokemonRepository.GetPokemons()
            .Where(pokemon => pokemon.Name.Trim().ToUpper() == pokemonDto.Name.Trim().ToUpper())
            .FirstOrDefault();

        if (pokemon != null)
        {
            ModelState.AddModelError("error", "Pokemon already exists");

            return StatusCode(StatusCodes.Status409Conflict, ModelState);
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var pokemonMap = _mapper.Map<Pokemon>(pokemonDto);

        var isPokemonCreated = _pokemonRepository.CreatePokemon(ownerId, categoryId, pokemonMap);

        if (!isPokemonCreated)
        {
            ModelState.AddModelError("error", "Something went wrong while saving");

            return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
        }

        return Ok("Successfully created");
    }
}
