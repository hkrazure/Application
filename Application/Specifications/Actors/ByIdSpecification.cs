using Domain.Models;
using System.Linq.Expressions;

namespace Application.Specifications.Actors;

public class ByIdSpecification(Guid Id) : ISpecification<Actor>
{
    public Expression<Func<Actor, bool>> Criteria => account => account.PublicId == Id;
}
