namespace Domain.ValueObjects;

public sealed record AccountNumber
{
    public string Value { get; }
    public AccountNumber()
    {
        Value = Guid.NewGuid().ToString();
    }
}
