using Application.Specifications;

namespace Infrastructure.Extensions;

internal static class QueryableExtensions
{
    public static IQueryable<TEntity> ApplySpecifications<TEntity>(this IQueryable<TEntity> inputQuery, IEnumerable<ISpecification<TEntity>> specifications)
    {
        foreach (var specification in specifications)
        {
            if (specification.Criteria is not null)
            {
                inputQuery = inputQuery.Where(specification.Criteria);
            }
        }

        return inputQuery;
    }
}
