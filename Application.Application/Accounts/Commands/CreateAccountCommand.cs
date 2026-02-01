using Application.Accounts.Responses;
using Application.Extensions.Requests;
using Domain.Enums;

namespace Application.Accounts.Commands;

public sealed record CreateAccountCommand(Guid ActorId, Currency CurrencyType) : ICommand<CreateAccountResponse>;
