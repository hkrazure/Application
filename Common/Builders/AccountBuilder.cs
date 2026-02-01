using Domain.Enums;
using Domain.Models;
using Domain.ValueObjects;

namespace Common.Builders;

public class AccountBuilder
{
    private Currency _currency = Currency.DKK;
    private decimal _balance = 0;
    private Actor _actor = new Person("test", "test");

    private Guid? _publicId;
    private AccountNumber? _accountNumber;

    public AccountBuilder WithCurrency(Currency currency)
    {
        _currency = currency;
        return this;
    }

    public AccountBuilder WithActor(Actor actor)
    {
        _actor = actor;
        return this;
    }

    public AccountBuilder WithBalance(decimal balance)
    {
        _balance = balance;
        return this;
    }

    public AccountBuilder WithPublicId(Guid publicId)
    {
        _publicId = publicId;
        return this;
    }

    public AccountBuilder WithAccountNumber(AccountNumber accountNumber)
    {
        _accountNumber = accountNumber;
        return this;
    }

    public Account Build()
    {
        var account = new Account(_currency, _actor);

        if (_publicId.HasValue)
        {
            typeof(Account).GetProperty(nameof(Account.PublicId))!
                .SetValue(account, _publicId.Value);
        }

        if (_accountNumber is not null)
        {
            typeof(Account).GetProperty(nameof(Account.AccountNumber))!
                .SetValue(account, _accountNumber);
        }

        if (_balance != 0)
        {
            typeof(Account).GetProperty(nameof(Account.Balance))!
                .SetValue(account, _balance);
        }

        return account;
    }
}
