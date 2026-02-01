using Application.Accounts.Commands;
using Application.Accounts.Handlers;
using Application.Exceptions;
using Application.Interfaces.Infrastructure;
using Application.Specifications;
using Common.Builders;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Models;
using Domain.ValueObjects;
using Moq;
using Xunit;

namespace UnitTests.Application.Handlers;

public class DepositAmountCommandHandlerTests
{
    private readonly Mock<IAccountRepository> _accountRepositoryMock;
    private readonly DepositAmountCommandHandler _handler;

    public DepositAmountCommandHandlerTests()
    {
        _accountRepositoryMock = new Mock<IAccountRepository>();
        _handler = new DepositAmountCommandHandler(_accountRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidAccountAndAmount_ShouldDepositSuccessfully()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var account = new AccountBuilder()
            .WithPublicId(accountId)
            .WithCurrency(Currency.DKK)
            .WithBalance(100m)
            .Build();
        
        var amount = new Amount(50m, Currency.DKK);
        var command = new DepositAmountCommand(accountId, amount);

        _accountRepositoryMock
            .Setup(x => x.Get(It.IsAny<IEnumerable<ISpecification<Account>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(150m, account.Balance);
        _accountRepositoryMock.Verify(x => x.Get(It.IsAny<IEnumerable<ISpecification<Account>>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentAccount_ShouldThrowEntityNotFoundException()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var amount = new Amount(50m, Currency.DKK);
        var command = new DepositAmountCommand(accountId, amount);

        _accountRepositoryMock
            .Setup(x => x.Get(It.IsAny<IEnumerable<ISpecification<Account>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Account?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() => 
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithNegativeAmount_ShouldThrowInvalidDepositAmountException()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var account = new AccountBuilder()
            .WithPublicId(accountId)
            .WithCurrency(Currency.DKK)
            .WithBalance(100m)
            .Build();
        
        var amount = new Amount(-50m, Currency.DKK);
        var command = new DepositAmountCommand(accountId, amount);

        _accountRepositoryMock
            .Setup(x => x.Get(It.IsAny<IEnumerable<ISpecification<Account>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidDepositAmountException>(() => 
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal(amount, exception.Amount);
        Assert.Equal(100m, account.Balance);
    }

    [Fact]
    public async Task Handle_WithWrongCurrency_ShouldThrowInvalidCurrencyException()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var account = new AccountBuilder()
            .WithPublicId(accountId)
            .WithCurrency(Currency.DKK)
            .WithBalance(100m)
            .Build();
        
        var amount = new Amount(50m, Currency.Undefined);
        var command = new DepositAmountCommand(accountId, amount);

        _accountRepositoryMock
            .Setup(x => x.Get(It.IsAny<IEnumerable<ISpecification<Account>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidCurrencyException>(() => 
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal(amount.Currency, exception.ActualCurrency);
        Assert.Equal(Currency.DKK, exception.ExpectedCurrency);
        Assert.Equal(100m, account.Balance);
    }

    [Fact]
    public async Task Handle_WithZeroAmount_ShouldNotChangeBalance()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var account = new AccountBuilder()
            .WithPublicId(accountId)
            .WithCurrency(Currency.DKK)
            .WithBalance(100m)
            .Build();
        
        var amount = new Amount(0m, Currency.DKK);
        var command = new DepositAmountCommand(accountId, amount);

        _accountRepositoryMock
            .Setup(x => x.Get(It.IsAny<IEnumerable<ISpecification<Account>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(100m, account.Balance);
    }
}