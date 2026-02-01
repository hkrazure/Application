using Application.Interfaces.Infrastructure;
using Application.Specifications;
using Domain.Models;
using Infrastructure.Contexts;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ActorRepository : IActorRepository
{
    private readonly AppDbContext _context;

    public ActorRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<Actor?> Get(IEnumerable<ISpecification<Actor>> specifications, CancellationToken cancellationToken)
    {
        return await _context.Actors
            .ApplySpecifications(specifications)
            .SingleOrDefaultAsync();
    }
}
