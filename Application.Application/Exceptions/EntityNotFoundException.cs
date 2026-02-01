using System;
using System.Collections.Generic;
using System.Linq;
namespace Application.Exceptions;

public sealed class EntityNotFoundException : Exception
{
    public string EntityName { get; }
    public Guid? EntityId { get; }

    public EntityNotFoundException(string entityName, Guid? entityId = null)
        : base(BuildMessage(entityName, entityId))
    {
        EntityName = entityName;
        EntityId = entityId;
    }

    private static string BuildMessage(string entityName, Guid? entityId)
    {
        if (entityId is null)
        {
            return $"{entityName} not found.";
        }

        return $"{entityName} with ID '{entityId}' not found.";
    }
}
