using Domain.Enums;
using Domain.Models;

namespace Application.Accounts.Responses;

public sealed record GetBalanceResponse
{
    public Guid AccountId { get; }
    public string AccountNumber { get; }
    public Currency Currency { get; }
    public decimal Balance { get; }
    public GetBalanceResponse(Account account)
    {
        AccountId = account.PublicId;
        AccountNumber = account.AccountNumber.Value;
        Currency = account.CurrencyType;
        Balance = account.Balance;
    }
}
