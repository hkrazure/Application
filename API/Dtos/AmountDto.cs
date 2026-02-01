using Domain.Enums;

namespace API.Dtos;

public sealed record AmountDto(decimal Value, CurrencyDto Currency);

