using MediatR;

namespace Application.Extensions.Requests;

public interface IQuery<out TResponse> : IRequest<TResponse>
{
}
