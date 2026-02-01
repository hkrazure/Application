using Application.Accounts.Commands;
using Application.Accounts.Handlers;
using Application.Exceptions;
using Application.Interfaces.Infrastructure;
using Application.Specifications;
using Domain.Enums;
using Domain.Models;
using Moq;
using Xunit;

namespace UnitTests.Application.Handlers;

public class CreateAccountCommandHandlerTests
{
    private readonly Mock<IAccountRepository> _accountRepositoryMock;
    private readonly Mock<IActorRepository> _actorRepositoryMock;
    private readonly CreateAccountCommandHandler _handler;

    public CreateAccountCommandHandlerTests()
    {
        _accountRepositoryMock = new Mock<IAccountRepository>();
        _actorRepositoryMock = new Mock<IActorRepository>();
        _handler = new CreateAccountCommandHandler(_accountRepositoryMock.Object, _actorRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidActorAndCurrency_ShouldCreateAccountAndReturnResponse()
    {
        // Arrange
        var actorId = Guid.NewGuid();
        var currency = Currency.DKK;
        var actor = new Person("John", "Doe");
        var command = new CreateAccountCommand(actorId, currency);

        _actorRepositoryMock
            .Setup(x => x.Get(It.IsAny<IEnumerable<ISpecification<Actor>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(actor);

        // Act
        var response = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(currency, response.Currency);
        Assert.Equal(0, response.Balance);
        Assert.Equal(actor.PublicId, response.ActorId);
        
        _actorRepositoryMock.Verify(x => x.Get(It.IsAny<IEnumerable<ISpecification<Actor>>>(), It.IsAny<CancellationToken>()), Times.Once);
        _accountRepositoryMock.Verify(x => x.Create(It.IsAny<Account>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentActor_ShouldThrowEntityNotFoundException()
    {
        // Arrange
        var actorId = Guid.NewGuid();
        var currency = Currency.DKK;
        var command = new CreateAccountCommand(actorId, currency);

        _actorRepositoryMock
            .Setup(x => x.Get(It.IsAny<IEnumerable<ISpecification<Actor>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Actor?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() => 
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal(nameof(Actor), exception.EntityName);
        Assert.Equal(actorId, exception.EntityId);
        
        _accountRepositoryMock.Verify(x => x.Create(It.IsAny<Account>()), Times.Never);
    }
}