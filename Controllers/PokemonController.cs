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
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult CreatePokemon([FromQuery] int ownerId, [FromQuery] int categoryId, [FromBody] PokemonDto pokemonDto)
    {
        if (pokemonDto == null)
        {
            return BadRequest(ModelState);
        }

        var pokemons = _pokemonRepository.GetPokemons()
            .Where(pokemon => pokemon.Name.Trim().ToUpper() == pokemonDto.Name.Trim().ToUpper())
            .FirstOrDefault();

        if (pokemons != null)
        {
            ModelState.AddModelError("", "Pokemon already exists");

            return StatusCode(StatusCodes.Status422UnprocessableEntity, ModelState);
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var pokemonMap = _mapper.Map<Pokemon>(pokemonDto);

        var isPokemonCreated = _pokemonRepository.CreatePokemon(ownerId, categoryId, pokemonMap);

        if (!isPokemonCreated)
        {
            ModelState.AddModelError("", "Something went wrong while saving");

            return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
        }

        return Ok("Successfully created");
    }
}
