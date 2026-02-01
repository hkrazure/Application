using Application.Extensions.Requests;
using Domain.ValueObjects;

namespace Application.Accounts.Commands;

public sealed record TransferAmountCommand(Guid FromAccountId, Guid ToAccountId, Amount Amount) : ICommand;
