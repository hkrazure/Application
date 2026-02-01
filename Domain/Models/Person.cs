using Domain.Exceptions;

namespace Domain.Models;

public class Person : Actor
{
#pragma warning disable CS8618 // Used by EF Core for materialization
    protected Person() { }
#pragma warning restore CS8618 
    public Person(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new IsNullOrWhiteSpaceException(nameof(FirstName), firstName);
        
        if (string.IsNullOrWhiteSpace(lastName))
            throw new IsNullOrWhiteSpaceException(nameof(LastName), lastName);

        FirstName = firstName;
        LastName = lastName;
    }

    public string FirstName { get; protected set; }
    public string LastName { get; protected set; }
}
