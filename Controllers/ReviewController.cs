using System.Net.Mime;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonApi.Dto;
using PokemonApi.Interfaces;
using PokemonApi.Models;

namespace PokemonApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReviewController : ControllerBase
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IPokemonRepository _pokemonRepository;
    private readonly IReviewerRepository _reviewerRepository;
    private readonly IMapper _mapper;

    public ReviewController(
        IReviewRepository reviewRepository,
        IPokemonRepository pokemonRepository,
        IReviewerRepository reviewerRepository,
        IMapper mapper)
    {
        _reviewRepository = reviewRepository;
        _pokemonRepository = pokemonRepository;
        _reviewerRepository = reviewerRepository;
        _mapper = mapper;
    }

    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(IEnumerable<ReviewDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult GetReviews()
    {
        var reviews = _mapper.Map<List<ReviewDto>>(_reviewRepository.GetReviews());

        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        return Ok(reviews);
    }

    [HttpGet("{reviewId:int}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ReviewDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetReview(int reviewId)
    {
        var isReviewExist = _reviewRepository.ReviewExists(reviewId);

        if (!isReviewExist)
        {
            return NotFound();
        }

        var review = _mapper.Map<ReviewDto>(_reviewRepository.GetReview(reviewId));

        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        return Ok(review);
    }

    [HttpGet("pokemon/{pokeId:int}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(IEnumerable<ReviewDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult GetReviewsOfAPokemon(int pokeId)
    {
        var reviews = _mapper.Map<List<ReviewDto>>(_reviewRepository.GetReviewsOfAPokemon(pokeId));

        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        return Ok(reviews);
    }

    [HttpPost]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult CreateReview([FromQuery] int reviewerId, [FromQuery] int pokeId, [FromBody] ReviewDto reviewDto)
    {
        if (reviewDto == null)
        {
            return BadRequest(ModelState);
        }

        var review = _reviewRepository.GetReviews()
            .Where(review => review.Title.Trim().ToUpper() == reviewDto.Title.Trim().ToUpper())
            .FirstOrDefault();

        if (review != null)
        {
            ModelState.AddModelError("error", "Review already exists");

            return StatusCode(StatusCodes.Status409Conflict, ModelState);
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var reviewMap = _mapper.Map<Review>(reviewDto);

        reviewMap.Pokemon = _pokemonRepository.GetPokemon(pokeId);
        reviewMap.Reviewer = _reviewerRepository.GetReviewer(reviewerId);

        var isReviewCreated = _reviewRepository.CreateReview(reviewMap);

        if (!isReviewCreated)
        {
            ModelState.AddModelError("error", "Something went wrong while saving");

            return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
        }

        return Ok("Successfully created");
    }

    [HttpPut("{reviewId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult UpdateReview(int reviewId, [FromBody] ReviewDto reviewDto)
    {
        if (reviewDto == null)
        {
            return BadRequest(ModelState);
        }

        if (reviewId != reviewDto.Id)
        {
            return BadRequest(ModelState);
        }

        var isReviewExist = _reviewRepository.ReviewExists(reviewId);

        if (!isReviewExist)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var reviewMap = _mapper.Map<Review>(reviewDto);

        var isReviewUpdated = _reviewRepository.UpdateReview(reviewMap);

        if (!isReviewUpdated)
        {
            ModelState.AddModelError("error", "Something went wrong updating review");

            return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
        }

        return NoContent();
    }

    [HttpDelete("{reviewId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult DeleteReview(int reviewId)
    {
        var isReviewExist = _reviewRepository.ReviewExists(reviewId);

        if (!isReviewExist)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var reviewToDelete = _reviewRepository.GetReview(reviewId);

        var isReviewDeleted = _reviewRepository.DeleteReview(reviewToDelete);

        if (!isReviewDeleted)
        {
            ModelState.AddModelError("error", "Something went wrong deleting review");

            return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
        }

        return NoContent();
    }

    [HttpDelete("/delete-reviews-by-reviewer/{reviewerId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult DeleteReviewsByReviewer(int reviewerId)
    {
        var isReviewerExist = _reviewerRepository.ReviewerExists(reviewerId);

        if (!isReviewerExist)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var reviewsToDelete = _reviewerRepository.GetReviewsByReviewer(reviewerId);

        var isReviewsDeleted = _reviewRepository.DeleteReviews(reviewsToDelete.ToList());

        if (!isReviewsDeleted)
        {
            ModelState.AddModelError("error", "Something went wrong deleting reviews");

            return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
        }

        return NoContent();
    }
}
