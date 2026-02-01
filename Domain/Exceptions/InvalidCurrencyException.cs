using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.Exceptions;

public sealed class InvalidCurrencyException : Exception
{
    public Currency ActualCurrency { get; }
    public Currency ExpectedCurrency { get; }
    public InvalidCurrencyException(Currency actualCurrency, Currency expectedCurrency) : base($"Invalid currency. {expectedCurrency} and {actualCurrency} do not match.")
    {
        ActualCurrency = actualCurrency;
        ExpectedCurrency = expectedCurrency;
    }
}
