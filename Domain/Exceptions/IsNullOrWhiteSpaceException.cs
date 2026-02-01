namespace Domain.Exceptions;

public sealed class IsNullOrWhiteSpaceException : Exception
{
    public string PropertyName { get; }
    public string? ProvidedValue { get; }

    public IsNullOrWhiteSpaceException(string propertyName, string? providedValue)
        : base(BuildMessage(propertyName, providedValue))
    {
        PropertyName = propertyName;
        ProvidedValue = providedValue;
    }

    private static string BuildMessage(string propertyName, string? providedValue)
    {
        if (string.IsNullOrWhiteSpace(providedValue))
        {
            return $"{propertyName} cannot be null or whitespace.";
        }

        return $"Invalid value for {propertyName}: '{providedValue}'.";
    }
}