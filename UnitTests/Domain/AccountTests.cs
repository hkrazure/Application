using Common.Builders;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Models;
using Domain.ValueObjects;
using Xunit;

namespace UnitTests.Domain;

public class AccountTests
{
    [Fact]
    public void Constructor_ShouldCreateAccountWithValidParameters()
    {
        // Arrange
        var currency = Currency.DKK;
        var actor = new Person("John", "Doe");

        // Act
        var account = new Account(currency, actor);

        // Assert
        Assert.NotEqual(Guid.Empty, account.PublicId);
        Assert.NotNull(account.AccountNumber);
        Assert.Equal(currency, account.CurrencyType);
        Assert.Equal(0, account.Balance);
        Assert.Equal(actor.InternalId, account.ActorId);
        Assert.Equal(actor, account.Actor);
    }

    [Fact]
    public void Deposit_WithValidAmount_ShouldIncreaseBalance()
    {
        // Arrange
        var account = new AccountBuilder()
            .WithCurrency(Currency.DKK)
            .Build();
        var amount = new Amount(100m, Currency.DKK);

        // Act
        account.Deposit(amount);

        // Assert
        Assert.Equal(100m, account.Balance);
    }

    [Fact]
    public void Deposit_WithMultipleDeposits_ShouldAccumulateBalance()
    {
        // Arrange
        var account = new AccountBuilder()
            .WithCurrency(Currency.DKK)
            .Build();
        var amount1 = new Amount(100m, Currency.DKK);
        var amount2 = new Amount(50m, Currency.DKK);

        // Act
        account.Deposit(amount1);
        account.Deposit(amount2);

        // Assert
        Assert.Equal(150m, account.Balance);
    }

    [Fact]
    public void Deposit_WithZeroAmount_ShouldNotChangeBalance()
    {
        // Arrange
        var account = new AccountBuilder()
            .WithCurrency(Currency.DKK)
            .WithBalance(100m)
            .Build();
        var amount = new Amount(0m, Currency.DKK);

        // Act
        account.Deposit(amount);

        // Assert
        Assert.Equal(100m, account.Balance);
    }

    [Fact]
    public void Deposit_WithNegativeAmount_ShouldThrowInvalidDepositAmountException()
    {
        // Arrange
        var account = new AccountBuilder()
            .WithCurrency(Currency.DKK)
            .Build();
        var amount = new Amount(-50m, Currency.DKK);

        // Act & Assert
        var exception = Assert.Throws<InvalidDepositAmountException>(() => account.Deposit(amount));
        Assert.Equal(amount, exception.Amount);
    }

    [Fact]
    public void Deposit_WithWrongCurrency_ShouldThrowInvalidCurrencyException()
    {
        // Arrange
        var account = new AccountBuilder()
            .WithCurrency(Currency.DKK)
            .Build();
        var amount = new Amount(100m, Currency.Undefined);

        // Act & Assert
        var exception = Assert.Throws<InvalidCurrencyException>(() => account.Deposit(amount));
        Assert.Equal(amount.Currency, exception.ActualCurrency);
        Assert.Equal(Currency.DKK, exception.ExpectedCurrency);
    }

    [Fact]
    public void Withdraw_WithValidAmount_ShouldDecreaseBalance()
    {
        // Arrange
        var account = new AccountBuilder()
            .WithCurrency(Currency.DKK)
            .WithBalance(200m)
            .Build();
        var amount = new Amount(100m, Currency.DKK);

        // Act
        account.Withdraw(amount);

        // Assert
        Assert.Equal(100m, account.Balance);
    }

    [Fact]
    public void Withdraw_WithExactBalance_ShouldSetBalanceToZero()
    {
        // Arrange
        var account = new AccountBuilder()
            .WithCurrency(Currency.DKK)
            .WithBalance(100m)
            .Build();
        var amount = new Amount(100m, Currency.DKK);

        // Act
        account.Withdraw(amount);

        // Assert
        Assert.Equal(0m, account.Balance);
    }

    [Fact]
    public void Withdraw_WithZeroAmount_ShouldNotChangeBalance()
    {
        // Arrange
        var account = new AccountBuilder()
            .WithCurrency(Currency.DKK)
            .WithBalance(100m)
            .Build();
        var amount = new Amount(0m, Currency.DKK);

        // Act
        account.Withdraw(amount);

        // Assert
        Assert.Equal(100m, account.Balance);
    }

    [Fact]
    public void Withdraw_WithNegativeAmount_ShouldThrowInvalidWithdrawAmountException()
    {
        // Arrange
        var account = new AccountBuilder()
            .WithCurrency(Currency.DKK)
            .WithBalance(100m)
            .Build();
        var amount = new Amount(-50m, Currency.DKK);

        // Act & Assert
        var exception = Assert.Throws<InvalidWithdrawAmountException>(() => account.Withdraw(amount));
        Assert.Equal(amount, exception.Amount);
    }

    [Fact]
    public void Withdraw_WithInsufficientFunds_ShouldThrowInsufficientFundsException()
    {
        // Arrange
        var account = new AccountBuilder()
            .WithCurrency(Currency.DKK)
            .WithBalance(50m)
            .Build();
        var amount = new Amount(100m, Currency.DKK);

        // Act & Assert
        Assert.Throws<InsufficientFundsException>(() => account.Withdraw(amount));
    }

    [Fact]
    public void Withdraw_WithWrongCurrency_ShouldThrowInvalidCurrencyException()
    {
        // Arrange
        var account = new AccountBuilder()
            .WithCurrency(Currency.DKK)
            .WithBalance(100m)
            .Build();
        var amount = new Amount(50m, Currency.Undefined);

        // Act & Assert
        var exception = Assert.Throws<InvalidCurrencyException>(() => account.Withdraw(amount));
        Assert.Equal(amount.Currency, exception.ActualCurrency);
        Assert.Equal(Currency.DKK, exception.ExpectedCurrency);
    }

    [Fact]
    public void GetBalance_ShouldReturnCorrectAmountWithCurrency()
    {
        // Arrange
        var account = new AccountBuilder()
            .WithCurrency(Currency.DKK)
            .WithBalance(150m)
            .Build();

        // Act
        var balance = account.GetBalance();

        // Assert
        Assert.Equal(150m, balance.Value);
        Assert.Equal(Currency.DKK, balance.Currency);
    }

    [Fact]
    public void GetBalance_WithZeroBalance_ShouldReturnZeroAmount()
    {
        // Arrange
        var account = new AccountBuilder()
            .WithCurrency(Currency.DKK)
            .WithBalance(0m)
            .Build();

        // Act
        var balance = account.GetBalance();

        // Assert
        Assert.Equal(0m, balance.Value);
        Assert.Equal(Currency.DKK, balance.Currency);
    }

    [Fact]
    public void ProtectedConstructor_ShouldBeCallableByEntityFramework()
    {
        // Arrange & Act
        var account = (Account)Activator.CreateInstance(typeof(Account), nonPublic: true)!;

        // Assert
        Assert.NotNull(account);
        Assert.Equal(Guid.Empty, account.PublicId);
        Assert.Equal(0, account.Balance);
    }
}
