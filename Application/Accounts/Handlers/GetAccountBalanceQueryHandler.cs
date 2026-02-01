using Application.Accounts.Queries;
using Application.Accounts.Responses;
using Application.Exceptions;
using Application.Interfaces.Infrastructure;
using Application.Specifications.Accounts;
using Domain.Models;
using MediatR;

namespace Application.Accounts.Handlers;

public sealed class GetAccountBalanceQueryHandler : IRequestHandler<GetBalanceQuery, GetBalanceResponse>
{
    private readonly IAccountRepository _accountRepository;

    public GetAccountBalanceQueryHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<GetBalanceResponse> Handle(GetBalanceQuery request, CancellationToken cancellationToken)
    {
        var specification = new ByIdSpecification(request.AccountId);
        var account = await _accountRepository.Get([specification], cancellationToken);

        if (account == null)
        {
            throw new EntityNotFoundException(nameof(Account), request.AccountId);
        }

        var response = new GetBalanceResponse(account);

        return response;
    }
}
