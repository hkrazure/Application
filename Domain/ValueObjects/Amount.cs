using Domain.Enums;

namespace Domain.ValueObjects;

public sealed record Amount
{
    public decimal Value { get; init; }

    public Currency Currency { get; init; }

    public Amount(decimal value, Currency currency)
    {
        Value = value;
        Currency = currency;
    }
}
