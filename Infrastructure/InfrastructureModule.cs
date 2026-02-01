using Application.Accounts.Handlers;
using Application.Interfaces.Infrastructure;
using Infrastructure.Behaviours;
using Infrastructure.Contexts;
using Infrastructure.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class InfrastructureModule
{
    private static SqliteConnection? _inMemoryConnection;

    public static IServiceCollection AddInfrastructureModule(this IServiceCollection services)
    {
        _inMemoryConnection = new SqliteConnection("DataSource=:memory:");
        _inMemoryConnection.Open();

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(_inMemoryConnection));

        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IActorRepository, ActorRepository>();

        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(typeof(CreateAccountCommandHandler).Assembly);

            configuration.AddOpenBehavior(typeof(DatabaseTransactionPipelineBehaviour<,>));
        });

        return services;
    }
}
