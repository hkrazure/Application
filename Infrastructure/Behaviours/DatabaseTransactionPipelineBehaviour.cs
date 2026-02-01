using Application.Extensions.Requests;
using Infrastructure.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Behaviours;

public class DatabaseTransactionPipelineBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{

    private readonly AppDbContext _context;
    private readonly ILogger<DatabaseTransactionPipelineBehaviour<TRequest, TResponse>> _logger;

    public DatabaseTransactionPipelineBehaviour(
        AppDbContext context,
        ILogger<DatabaseTransactionPipelineBehaviour<TRequest, TResponse>> logger)
    {
        _context = context;
        _logger = logger;
    }
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var isCommand = request is ICommand || request is ICommand<TResponse>;

        if (!isCommand)
        {
            return await next();
        }

        IDbContextTransaction? transaction = null;
        if (_context.Database.CurrentTransaction is null)
        {
            transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        try
        {
            var response = await next();
            if (transaction is not null)
            {
                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }

            return response;
        }
        catch (Exception)
        {
            _logger.LogTrace($"Rolling back database transaction due to exception, request type: {typeof(TRequest)}");
            if (transaction is not null)
                await transaction.RollbackAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (transaction is not null)
                await transaction.DisposeAsync();
        }
    }
}
