using Domain.Models;
using Domain.ValueObjects;
using System.Linq.Expressions;

namespace Application.Specifications.Accounts;

public class ByAccountNumberSpecification(AccountNumber AccountNumber) : ISpecification<Account>
{
    public Expression<Func<Account, bool>> Criteria => account => account.AccountNumber == AccountNumber;
}
