using Application.Accounts.Commands;
using Application.Exceptions;
using Application.Interfaces.Infrastructure;
using Application.Specifications.Accounts;
using Domain.Exceptions;
using Domain.Models;
using MediatR;

namespace Application.Accounts.Handlers;

public sealed class TransferAmountCommandHandler : IRequestHandler<TransferAmountCommand>
{
    private readonly IAccountRepository _accountRepository;

    public TransferAmountCommandHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task Handle(TransferAmountCommand request, CancellationToken cancellationToken)
    {
        var fromAccountSpecificaiton = new ByIdSpecification(request.FromAccountId);
        var fromAccount = await _accountRepository.Get([fromAccountSpecificaiton], cancellationToken);

        if(fromAccount is null)
        {
            throw new EntityNotFoundException(nameof(Account), request.FromAccountId);
        }

        var toAccountSpecification = new ByIdSpecification(request.ToAccountId);
        var toAccount = await _accountRepository.Get([toAccountSpecification], cancellationToken);

        if(toAccount is null)
        {
            throw new EntityNotFoundException(nameof(Account), request.ToAccountId);
        }

        if(fromAccount.CurrencyType != toAccount.CurrencyType)
        {
            throw new InvalidCurrencyException(fromAccount.CurrencyType, toAccount.CurrencyType);
        }

        fromAccount.Withdraw(request.Amount);
        toAccount.Deposit(request.Amount);
    }
}
