using API.Dtos;
using Domain.Enums;

namespace API.Extensions;

public static class CurrencyDtoExtensions
{
    public static Currency ToDomain(this CurrencyDto currencyDto)
    {
        return currencyDto switch
        {
            Dtos.CurrencyDto.DKK => Currency.DKK,
            _ => throw new ArgumentOutOfRangeException(nameof(currencyDto), currencyDto, "Unsupported currency")
        };
    }

    public static CurrencyDto ToDto(this Currency currency)
    {
        return currency switch
        {
            Currency.DKK => CurrencyDto.DKK,
            _ => throw new ArgumentOutOfRangeException(nameof(currency), currency, "Unsupported currency for DTO")
        };
    }
}