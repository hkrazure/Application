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

public class TransferAmountCommandHandlerTests
{
    private readonly Mock<IAccountRepository> _accountRepositoryMock;
    private readonly TransferAmountCommandHandler _handler;

    public TransferAmountCommandHandlerTests()
    {
        _accountRepositoryMock = new Mock<IAccountRepository>();
        _handler = new TransferAmountCommandHandler(_accountRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidAccountsAndAmount_ShouldTransferSuccessfully()
    {
        // Arrange
        var fromAccountId = Guid.NewGuid();
        var toAccountId = Guid.NewGuid();
        
        var fromAccount = new AccountBuilder()
            .WithPublicId(fromAccountId)
            .WithCurrency(Currency.DKK)
            .WithBalance(200m)
            .Build();
        
        var toAccount = new AccountBuilder()
            .WithPublicId(toAccountId)
            .WithCurrency(Currency.DKK)
            .WithBalance(50m)
            .Build();
        
        var amount = new Amount(100m, Currency.DKK);
        var command = new TransferAmountCommand(fromAccountId, toAccountId, amount);

        _accountRepositoryMock
            .SetupSequence(x => x.Get(It.IsAny<IEnumerable<ISpecification<Account>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fromAccount)
            .ReturnsAsync(toAccount);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(100m, fromAccount.Balance);
        Assert.Equal(150m, toAccount.Balance);
        _accountRepositoryMock.Verify(x => x.Get(It.IsAny<IEnumerable<ISpecification<Account>>>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task Handle_WithNonExistentFromAccount_ShouldThrowEntityNotFoundException()
    {
        // Arrange
        var fromAccountId = Guid.NewGuid();
        var toAccountId = Guid.NewGuid();
        var amount = new Amount(100m, Currency.DKK);
        var command = new TransferAmountCommand(fromAccountId, toAccountId, amount);

        _accountRepositoryMock
            .Setup(x => x.Get(It.IsAny<IEnumerable<ISpecification<Account>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Account?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() => 
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal(nameof(Account), exception.EntityName);
        Assert.Equal(fromAccountId, exception.EntityId);
    }

    [Fact]
    public async Task Handle_WithNonExistentToAccount_ShouldThrowEntityNotFoundException()
    {
        // Arrange
        var fromAccountId = Guid.NewGuid();
        var toAccountId = Guid.NewGuid();
        
        var fromAccount = new AccountBuilder()
            .WithPublicId(fromAccountId)
            .WithCurrency(Currency.DKK)
            .WithBalance(200m)
            .Build();
        
        var amount = new Amount(100m, Currency.DKK);
        var command = new TransferAmountCommand(fromAccountId, toAccountId, amount);

        _accountRepositoryMock
            .SetupSequence(x => x.Get(It.IsAny<IEnumerable<ISpecification<Account>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fromAccount)
            .ReturnsAsync((Account?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() => 
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal(nameof(Account), exception.EntityName);
        Assert.Equal(toAccountId, exception.EntityId);
        Assert.Equal(200m, fromAccount.Balance);
    }

    [Fact]
    public async Task Handle_WithMismatchedCurrencies_ShouldThrowInvalidCurrencyException()
    {
        // Arrange
        var fromAccountId = Guid.NewGuid();
        var toAccountId = Guid.NewGuid();
        
        var fromAccount = new AccountBuilder()
            .WithPublicId(fromAccountId)
            .WithCurrency(Currency.DKK)
            .WithBalance(200m)
            .Build();
        
        var toAccount = new AccountBuilder()
            .WithPublicId(toAccountId)
            .WithCurrency(Currency.Undefined)
            .WithBalance(50m)
            .Build();
        
        var amount = new Amount(100m, Currency.DKK);
        var command = new TransferAmountCommand(fromAccountId, toAccountId, amount);

        _accountRepositoryMock
            .SetupSequence(x => x.Get(It.IsAny<IEnumerable<ISpecification<Account>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fromAccount)
            .ReturnsAsync(toAccount);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidCurrencyException>(() => 
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal(200m, fromAccount.Balance);
        Assert.Equal(50m, toAccount.Balance);
    }

    [Fact]
    public async Task Handle_WithInsufficientFunds_ShouldThrowInsufficientFundsException()
    {
        // Arrange
        var fromAccountId = Guid.NewGuid();
        var toAccountId = Guid.NewGuid();
        
        var fromAccount = new AccountBuilder()
            .WithPublicId(fromAccountId)
            .WithCurrency(Currency.DKK)
            .WithBalance(50m)
            .Build();
        
        var toAccount = new AccountBuilder()
            .WithPublicId(toAccountId)
            .WithCurrency(Currency.DKK)
            .WithBalance(50m)
            .Build();
        
        var amount = new Amount(100m, Currency.DKK);
        var command = new TransferAmountCommand(fromAccountId, toAccountId, amount);

        _accountRepositoryMock
            .SetupSequence(x => x.Get(It.IsAny<IEnumerable<ISpecification<Account>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fromAccount)
            .ReturnsAsync(toAccount);

        // Act & Assert
        await Assert.ThrowsAsync<InsufficientFundsException>(() => 
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal(50m, fromAccount.Balance);
        Assert.Equal(50m, toAccount.Balance);
    }

    [Fact]
    public async Task Handle_WithExactBalance_ShouldTransferSuccessfully()
    {
        // Arrange
        var fromAccountId = Guid.NewGuid();
        var toAccountId = Guid.NewGuid();
        
        var fromAccount = new AccountBuilder()
            .WithPublicId(fromAccountId)
            .WithCurrency(Currency.DKK)
            .WithBalance(100m)
            .Build();
        
        var toAccount = new AccountBuilder()
            .WithPublicId(toAccountId)
            .WithCurrency(Currency.DKK)
            .WithBalance(0m)
            .Build();
        
        var amount = new Amount(100m, Currency.DKK);
        var command = new TransferAmountCommand(fromAccountId, toAccountId, amount);

        _accountRepositoryMock
            .SetupSequence(x => x.Get(It.IsAny<IEnumerable<ISpecification<Account>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fromAccount)
            .ReturnsAsync(toAccount);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(0m, fromAccount.Balance);
        Assert.Equal(100m, toAccount.Balance);
    }

    [Fact]
    public async Task Handle_WithWrongAmountCurrency_ShouldThrowInvalidCurrencyException()
    {
        // Arrange
        var fromAccountId = Guid.NewGuid();
        var toAccountId = Guid.NewGuid();
        
        var fromAccount = new AccountBuilder()
            .WithPublicId(fromAccountId)
            .WithCurrency(Currency.DKK)
            .WithBalance(200m)
            .Build();
        
        var toAccount = new AccountBuilder()
            .WithPublicId(toAccountId)
            .WithCurrency(Currency.DKK)
            .WithBalance(50m)
            .Build();
        
        var amount = new Amount(100m, Currency.Undefined);
        var command = new TransferAmountCommand(fromAccountId, toAccountId, amount);

        _accountRepositoryMock
            .SetupSequence(x => x.Get(It.IsAny<IEnumerable<ISpecification<Account>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fromAccount)
            .ReturnsAsync(toAccount);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidCurrencyException>(() => 
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal(200m, fromAccount.Balance);
        Assert.Equal(50m, toAccount.Balance);
    }
}