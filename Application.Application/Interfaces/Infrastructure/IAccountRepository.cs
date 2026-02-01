using Application.Specifications;
using Domain.Models;

namespace Application.Interfaces.Infrastructure;

public interface IAccountRepository
{
    public Task<Account?> Get(IEnumerable<ISpecification<Account>> specifications, CancellationToken cancellationToken);

    public void Create(Account account);
}
