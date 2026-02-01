using Domain.ValueObjects;

namespace API.Dtos;

public sealed record TransferAmountDto(Guid ToAccountId, AmountDto Amount);
