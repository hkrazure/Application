using Application.Accounts.Handlers;
using Application.Accounts.Queries;
using Application.Exceptions;
using Application.Interfaces.Infrastructure;
using Application.Specifications;
using Common.Builders;
using Domain.Enums;
using Domain.Models;
using Moq;
using Xunit;

namespace UnitTests.Application.Handlers;

public class GetAccountBalanceQueryHandlerTests
{
    private readonly Mock<IAccountRepository> _accountRepositoryMock;
    private readonly GetAccountBalanceQueryHandler _handler;

    public GetAccountBalanceQueryHandlerTests()
    {
        _accountRepositoryMock = new Mock<IAccountRepository>();
        _handler = new GetAccountBalanceQueryHandler(_accountRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithExistingAccount_ShouldReturnBalanceResponse()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var account = new AccountBuilder()
            .WithPublicId(accountId)
            .WithCurrency(Currency.DKK)
            .WithBalance(150m)
            .Build();
        
        var query = new GetBalanceQuery(accountId);

        _accountRepositoryMock
            .Setup(x => x.Get(It.IsAny<IEnumerable<ISpecification<Account>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);

        // Act
        var response = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(accountId, response.AccountId);
        Assert.Equal(150m, response.Balance);
        Assert.Equal(Currency.DKK, response.Currency);
        
        _accountRepositoryMock.Verify(x => x.Get(It.IsAny<IEnumerable<ISpecification<Account>>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentAccount_ShouldThrowEntityNotFoundException()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var query = new GetBalanceQuery(accountId);

        _accountRepositoryMock
            .Setup(x => x.Get(It.IsAny<IEnumerable<ISpecification<Account>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Account?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() => 
            _handler.Handle(query, CancellationToken.None));

        Assert.Equal(nameof(Account), exception.EntityName);
        Assert.Equal(accountId, exception.EntityId);
    }
}