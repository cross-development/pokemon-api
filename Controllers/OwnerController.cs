using System.Net.Mime;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonApi.Dto;
using PokemonApi.Interfaces;
using PokemonApi.Models;

namespace PokemonApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OwnerController : ControllerBase
{
    private readonly IOwnerRepository _ownerRepository;
    private readonly ICountryRepository _countryRepository;
    private readonly IMapper _mapper;

    public OwnerController(IOwnerRepository ownerRepository, ICountryRepository countryRepository, IMapper mapper)
    {
        _ownerRepository = ownerRepository;
        _countryRepository = countryRepository;
        _mapper = mapper;
    }

    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(IEnumerable<OwnerDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult GetOwners()
    {
        var owners = _mapper.Map<List<OwnerDto>>(_ownerRepository.GetOwners());

        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        return Ok(owners);
    }

    [HttpGet("{ownerId:int}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(OwnerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetOwner(int ownerId)
    {
        var isOwnerExist = _ownerRepository.OwnerExists(ownerId);

        if (!isOwnerExist)
        {
            return NotFound();
        }

        var owner = _mapper.Map<OwnerDto>(_ownerRepository.GetOwner(ownerId));

        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        return Ok(owner);
    }

    [HttpGet("{ownerId:int}/pokemon")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(IEnumerable<PokemonDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetPokemonByOwner(int ownerId)
    {
        var isOwnerExist = _ownerRepository.OwnerExists(ownerId);

        if (!isOwnerExist)
        {
            return NotFound();
        }

        var pokemon = _mapper.Map<List<PokemonDto>>(_ownerRepository.GetPokemonByOwner(ownerId));

        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        return Ok(pokemon);
    }

    [HttpGet("{pokeId:int}/owners")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(IEnumerable<OwnerDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetOwnerOfAPokemon(int pokeId)
    {
        var isOwnerExist = _ownerRepository.OwnerExists(pokeId);

        if (!isOwnerExist)
        {
            return NotFound();
        }

        var owners = _mapper.Map<List<OwnerDto>>(_ownerRepository.GetOwnerOfAPokemon(pokeId));

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
    public IActionResult CreateOwner([FromQuery] int countryId, [FromBody] OwnerDto ownerDto)
    {
        if (ownerDto == null)
        {
            return BadRequest(ModelState);
        }

        var owner = _ownerRepository.GetOwners()
            .Where(owner => owner.LastName.Trim().ToUpper() == ownerDto.LastName.Trim().ToUpper())
            .FirstOrDefault();

        if (owner != null)
        {
            ModelState.AddModelError("error", "Owner already exists");

            return StatusCode(StatusCodes.Status409Conflict, ModelState);
        }

        var ownerMap = _mapper.Map<Owner>(ownerDto);

        ownerMap.Country = _countryRepository.GetCountry(countryId);

        var isOwnerCreated = _ownerRepository.CreateOwner(ownerMap);

        if (!isOwnerCreated)
        {
            ModelState.AddModelError("error", "Something went wrong while saving");

            return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
        }

        return Ok("Successfully created");
    }
}
