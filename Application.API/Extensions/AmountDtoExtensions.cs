using API.Dtos;
using API.Extensions;
using Domain.ValueObjects;

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