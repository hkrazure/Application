using Application.Extensions.Requests;
using Domain.ValueObjects;

namespace Application.Accounts.Commands;

public sealed record DepositAmountCommand(Guid AccountId, Amount Amount) : ICommand;
