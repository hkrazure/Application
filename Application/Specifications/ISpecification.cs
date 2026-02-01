using System.Linq.Expressions;

namespace Application.Specifications;

public interface ISpecification<TEntity>
{
    Expression<Func<TEntity, bool>> Criteria { get; }
}
