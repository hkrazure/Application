using Application.Accounts.Commands;
using Application.Exceptions;
using Application.Interfaces.Infrastructure;
using Application.Specifications.Accounts;
using Domain.Models;
using MediatR;

namespace Application.Accounts.Handlers;

public sealed class DepositAmountCommandHandler : IRequestHandler<DepositAmountCommand>
{
    private readonly IAccountRepository _accountRepository;

    public DepositAmountCommandHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }
    public async Task Handle(DepositAmountCommand request, CancellationToken cancellationToken)
    {
        var specification = new ByIdSpecification(request.AccountId);
        var account = await _accountRepository.Get([specification], cancellationToken);

        if (account is null)
        {
            throw new EntityNotFoundException(nameof(Account), request.AccountId);
        }

        account.Deposit(request.Amount);
    }
}
