namespace Domain.Models;

public abstract class Actor
{
    public int InternalId { get; protected set; }

    public Guid PublicId { get; protected set; }

    protected Actor()
    {
        PublicId = Guid.NewGuid();
    }

    public static T? TryCastActor<T>(Actor actor) where T : Actor
    {
        return actor as T;
    }
}
