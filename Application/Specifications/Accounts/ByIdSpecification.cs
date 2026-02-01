using Domain.Models;
using System.Linq.Expressions;

namespace Application.Specifications.Accounts;

public class ByIdSpecification(Guid Id) : ISpecification<Account>
{
    public Expression<Func<Account, bool>> Criteria => account => account.PublicId == Id;
}
