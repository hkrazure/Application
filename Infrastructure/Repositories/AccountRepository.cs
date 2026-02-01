using Application.Interfaces.Infrastructure;
using Application.Specifications;
using Domain.Models;
using Infrastructure.Contexts;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class AccountRepository : IAccountRepository
{
    public AppDbContext _context { get; }

    public AccountRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Account?> Get(IEnumerable<ISpecification<Account>> specifications, CancellationToken cancellationToken) 
    { 
        return await _context.Accounts
            .ApplySpecifications(specifications)
            .Include(a => a.Actor)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public void Create(Account account)
    {
        _context.Accounts.Add(account);
    }
}
