using Domain.ValueObjects;

namespace Domain.Exceptions;

public sealed class InvalidDepositAmountException : Exception
{
    public Amount Amount { get; }
    public InvalidDepositAmountException(Amount amount) : base($"Invalid deposit amount: {amount.Value} {amount.Currency}. Deposit amount must be greater than zero.")
    {
        Amount = amount;
    }
}
