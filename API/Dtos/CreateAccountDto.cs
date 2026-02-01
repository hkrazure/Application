namespace API.Dtos;

public sealed record CreateAccountDto(Guid ActorId, CurrencyDto CurrencyType);
