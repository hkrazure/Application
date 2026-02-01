using Application.Specifications;
using Domain.Models;

namespace Application.Interfaces.Infrastructure;

public interface IActorRepository
{
    public Task<Actor?> Get(IEnumerable<ISpecification<Actor>> specifications, CancellationToken cancellationToken);
}
