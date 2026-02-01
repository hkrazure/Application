using API.Dtos;
using Domain.ValueObjects;

namespace API.Extensions;

public static class AmountDtoExtensions
{
    public static AmountDto ToDto(this Amount amount)
    {
        return new AmountDto(amount.Value, amount.Currency.ToDto());
    }
    public static Amount ToDomain(this AmountDto amountDto)
    {
        return new Amount(amountDto.Value, amountDto.Currency.ToDomain());
    }
}