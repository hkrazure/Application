using MediatR;

namespace Application.Extensions.Requests;

public interface ICommand<out TResponse> : IRequest<TResponse>
{
}

public interface ICommand : IRequest
{
}
