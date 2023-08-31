using System.Net.Mime;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonApi.Dto;
using PokemonApi.Interfaces;
using PokemonApi.Models;

namespace PokemonApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReviewerController : ControllerBase
{
    private readonly IReviewerRepository _reviewerRepository;
    private readonly IMapper _mapper;

    public ReviewerController(IReviewerRepository reviewerRepository, IMapper mapper)
    {
        _reviewerRepository = reviewerRepository;
        _mapper = mapper;
    }

    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(IEnumerable<ReviewerDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult GetReviewers()
    {
        var reviewers = _mapper.Map<List<ReviewerDto>>(_reviewerRepository.GetReviewers());

        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        return Ok(reviewers);
    }

    [HttpGet("{reviewerId:int}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ReviewerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetReviewer(int reviewerId)
    {
        var isReviewerExist = _reviewerRepository.ReviewerExists(reviewerId);

        if (!isReviewerExist)
        {
            return NotFound();
        }

        var reviewer = _mapper.Map<ReviewerDto>(_reviewerRepository.GetReviewer(reviewerId));

        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        return Ok(reviewer);
    }

    [HttpGet("{reviewerId:int}/reviews")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(IEnumerable<ReviewDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetReviewsByReviewer(int reviewerId)
    {
        var isReviewerExist = _reviewerRepository.ReviewerExists(reviewerId);

        if (!isReviewerExist)
        {
            return NotFound();
        }

        var reviews = _mapper.Map<List<ReviewDto>>(_reviewerRepository.GetReviewsByReviewer(reviewerId));

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
    public IActionResult CreateReviewer([FromBody] ReviewerDto reviewerDto)
    {
        if (reviewerDto == null)
        {
            return BadRequest(ModelState);
        }

        var reviewer = _reviewerRepository.GetReviewers()
            .Where(reviewer => reviewer.LastName.Trim().ToUpper() == reviewerDto.LastName.TrimEnd().ToUpper())
            .FirstOrDefault();

        if (reviewer != null)
        {
            ModelState.AddModelError("error", "Reviewer already exists");

            return StatusCode(StatusCodes.Status409Conflict, ModelState);
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState); 
        }

        var reviewerMap = _mapper.Map<Reviewer>(reviewerDto);

        var isReviewerCreated = _reviewerRepository.CreateReviewer(reviewerMap);

        if (!isReviewerCreated)
        {
            ModelState.AddModelError("error", "Something went wrong while saving");

            return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
        }

        return Ok("Successfully created");
    }
}
