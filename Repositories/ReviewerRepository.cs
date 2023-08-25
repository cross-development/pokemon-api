using Microsoft.EntityFrameworkCore;
using PokemonApi.Data;
using PokemonApi.Interfaces;
using PokemonApi.Models;

namespace PokemonApi.Repositories;

public class ReviewerRepository : IReviewerRepository
{
    private readonly DataContext _context;

    public ReviewerRepository(DataContext context)
    {
        _context = context;
    }

    public ICollection<Reviewer> GetReviewers()
    {
        return _context.Reviewers.Include(reviewer => reviewer.Reviews).ToList();
    }

    public Reviewer GetReviewer(int reviewerId)
    {
        return _context.Reviewers
            .Where(reviewer => reviewer.Id == reviewerId)
            .Include(reviewer => reviewer.Reviews)
            .FirstOrDefault();
    }

    public ICollection<Review> GetReviewsByReviewer(int reviewerId)
    {
        return _context.Reviewers
            .Where(reviewer => reviewer.Id == reviewerId)
            .SelectMany(reviewer => reviewer.Reviews)
            .ToList();
    }

    public bool ReviewerExists(int reviewerId)
    {
        return _context.Reviewers.Any(reviewer => reviewer.Id == reviewerId);
    }
}