using Domain.ValueObjects;

namespace Domain.Exceptions;

public sealed class InvalidWithdrawAmountException : Exception
{
    public Amount Amount { get; }
    public InvalidWithdrawAmountException(Amount amount) : base($"Invalid withdraw amount: {amount.Value} {amount.Currency}. Withdraw amount must be greater than zero.")
    {
        Amount = amount;
    }
}
