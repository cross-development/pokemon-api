using System.Net.Mime;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonApi.Dto;
using PokemonApi.Interfaces;
using PokemonApi.Models;

namespace PokemonApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public CategoryController(ICategoryRepository categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(IEnumerable<CategoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult GetCategories()
    {
        var categories = _mapper.Map<List<CategoryDto>>(_categoryRepository.GetCategories());

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok(categories);
    }

    [HttpGet("{categoryId:int}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetCategory(int categoryId)
    {
        var isCategoryExist = _categoryRepository.CategoriesExists(categoryId);

        if (!isCategoryExist)
        {
            return NotFound();
        }

        var category = _mapper.Map<CategoryDto>(_categoryRepository.GetCategory(categoryId));

        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        return Ok(category);
    }

    [HttpGet("pokemon/{categoryId:int}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(IEnumerable<PokemonDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult GetPokemonByCategoryId(int categoryId)
    {
        var pokemons = _mapper.Map<List<PokemonDto>>(_categoryRepository.GetPokemonByCategoryId(categoryId));

        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        return Ok(pokemons);
    }

    [HttpPost]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult CreateCategory([FromBody] CategoryDto categoryDto)
    {
        if (categoryDto == null)
        {
            return BadRequest(ModelState);
        }

        var category = _categoryRepository.GetCategories()
            .Where(category => category.Name.Trim().ToUpper() == categoryDto.Name.Trim().ToUpper())
            .FirstOrDefault();

        if (category != null)
        {
            ModelState.AddModelError("error", "Category already exists");

            return StatusCode(StatusCodes.Status409Conflict, ModelState);
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var categoryMap = _mapper.Map<Category>(categoryDto);

        var isCategoryCreated = _categoryRepository.CreateCategory(categoryMap);

        if (!isCategoryCreated)
        {
            ModelState.AddModelError("error", "Something went wrong while saving");

            return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
        }

        return Ok("Successfully created");
    }

    [HttpPut("{categoryId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult UpdateCategory(int categoryId, [FromBody] CategoryDto categoryDto)
    {
        if (categoryDto == null)
        {
            return BadRequest(ModelState);
        }

        if (categoryId != categoryDto.Id)
        {
            return BadRequest(ModelState);
        }

        var isCategoryExist = _categoryRepository.CategoriesExists(categoryId);

        if (!isCategoryExist)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var categoryMap = _mapper.Map<Category>(categoryDto);

        var isCategoryUpdated = _categoryRepository.UpdateCategory(categoryMap);

        if (!isCategoryUpdated)
        {
            ModelState.AddModelError("error", "Something went wrong updating category");

            return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
        }

        return NoContent();
    }

}
