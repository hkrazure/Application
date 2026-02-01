using Application.Accounts.Commands;
using Application.Accounts.Responses;
using Application.Exceptions;
using Application.Interfaces.Infrastructure;
using Application.Specifications.Actors;
using Domain.Enums;
using Domain.Models;
using MediatR;

namespace Application.Accounts.Handlers;

public sealed class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, CreateAccountResponse>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IActorRepository _actorRepository;

    public CreateAccountCommandHandler(IAccountRepository accountRepository, IActorRepository actorRepository)
    {
        _accountRepository = accountRepository;
        _actorRepository = actorRepository;
    }
    public async Task<CreateAccountResponse> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        var (actorId, currency) = request;

        var specfication = new ByIdSpecification(actorId);
        var actor = await _actorRepository.Get([specfication], cancellationToken);

        if (actor is null)
        {
            throw new EntityNotFoundException(nameof(Actor), actorId); 
        }

        var account = new Account(currency, actor);

        _accountRepository.Create(account);

        var response = new CreateAccountResponse(account);

        return response;
    }
}
