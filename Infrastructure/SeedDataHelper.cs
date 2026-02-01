using Domain.Models;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class SeedDataHelper
{
    public static readonly Guid Actor1Id = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid Actor2Id = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public static readonly Guid Actor3Id = Guid.Parse("33333333-3333-3333-3333-333333333333");

    public static void SeedInMemoryData(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // For SQLite, use EnsureCreated or migrations
        context.Database.EnsureCreated();

        if (!context.Actors.Any())
        {
            var actor1 = new Person("John", "Smith");
            var actor2 = new Person("Emma", "Johnson");
            var actor3 = new Person("Michael", "Williams");

            typeof(Actor).GetProperty(nameof(Actor.InternalId))!.SetValue(actor1, 1);
            typeof(Actor).GetProperty(nameof(Actor.InternalId))!.SetValue(actor2, 2);
            typeof(Actor).GetProperty(nameof(Actor.InternalId))!.SetValue(actor3, 3);

            typeof(Actor).GetProperty(nameof(Actor.PublicId))!.SetValue(actor1, Actor1Id);
            typeof(Actor).GetProperty(nameof(Actor.PublicId))!.SetValue(actor2, Actor2Id);
            typeof(Actor).GetProperty(nameof(Actor.PublicId))!.SetValue(actor3, Actor3Id);

            context.Actors.AddRange(actor1, actor2, actor3);
            context.SaveChanges();
        }
    }
}
