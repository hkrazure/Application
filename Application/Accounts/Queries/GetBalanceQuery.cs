using Application.Accounts.Responses;
using Application.Extensions.Requests;

namespace Application.Accounts.Queries;

public sealed record GetBalanceQuery(Guid AccountId) : IQuery<GetBalanceResponse>;
