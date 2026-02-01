using Domain.Enums;
using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Models;

public class Account
{
    public int InternalId { get; protected set; }
    public Guid PublicId { get; protected set; }
    public AccountNumber AccountNumber { get; protected set; }
    public Currency CurrencyType { get; protected set; }
    public decimal Balance { get; protected set; }
    public int ActorId { get; protected set; }
    public Actor Actor { get; protected set; }

#pragma warning disable CS8618 // Used by EF core for materialization
    protected Account() { }
#pragma warning restore CS8618 
    public Account(Currency currency, Actor actor)
    {      
        PublicId = Guid.NewGuid();
        AccountNumber = new AccountNumber(); 
        CurrencyType = currency;
        ActorId = actor.InternalId;
        Actor = actor;
        Balance = 0;
    }

    public void Deposit(Amount amount)
    {
        ValidateCurrency(amount);

        if (amount.Value == 0)
        {
            return;
        }

        if (amount.Value < 0)
        {
            throw new InvalidDepositAmountException(amount);
        }

        Balance += amount.Value;
    }

    public void Withdraw(Amount amount)
    {
        ValidateCurrency(amount);

        if(amount.Value == 0)
        {
            return;
        }

        if (amount.Value < 0)
        {
            throw new InvalidWithdrawAmountException(amount);
        }

        if (Balance - amount.Value < 0)
        {
            throw new InsufficientFundsException();
        }     
            
        Balance -= amount.Value;
    }

    public Amount GetBalance()
    {
        return new Amount(Balance, CurrencyType);
    }

    private void ValidateCurrency(Amount amount)
    {
        if (amount.Currency != CurrencyType)
        {
            throw new InvalidCurrencyException(amount.Currency, CurrencyType);
        }
    }
}
