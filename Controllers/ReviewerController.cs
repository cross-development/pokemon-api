using System.Net.Mime;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonApi.Dto;
using PokemonApi.Interfaces;

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
}
