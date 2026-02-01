using Domain.Enums;
using Domain.Models;

namespace Application.Accounts.Responses;

public sealed record CreateAccountResponse
{
    public Guid Id { get; }
    public string AccountNumber { get; }
    public Currency Currency { get; }
    public decimal Balance { get; }
    public Guid ActorId { get; }
    
    public CreateAccountResponse(Account account)
    {
        Id = account.PublicId;
        AccountNumber = account.AccountNumber.Value;
        Currency = account.CurrencyType;
        Balance = account.Balance;
        ActorId = account.Actor.PublicId;
    }
}
