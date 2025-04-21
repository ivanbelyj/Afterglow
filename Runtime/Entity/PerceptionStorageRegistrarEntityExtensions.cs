using System;
using System.Collections.Generic;
using UnityEngine;

public static class PerceptionStorageRegistrarEntityExtensions
{
    /// <summary>
    /// Ensures that storage for the specified entity is registered
    /// and then gets relevant perceptions
    /// </summary>
    public static List<PerceptionEntry> GetPerceptionsFromRegisteredStorageForEntity(
        this PerceptionStorageRegistrar registrar,
        Guid entityId)
    {
        var perceptionSource = registrar.EnsureStorageExistsForEntity(entityId);
        return registrar.segregatedMemoryManager.GetPerceptions(perceptionSource);
    }

    private static IPerceptionSource EnsureStorageExistsForEntity(
        this PerceptionStorageRegistrar registrar,
        Guid entityId)
    {
        var key = GetPerceptionSourceKeyByEntityId(entityId);

        return registrar.EnsureRegistered(
            key,
            CreatePerceptionTrackerLazy(entityId)
        );
    }

    private static Lazy<PerceptionCollectionTrackerBase<List<PerceptionEntry>, IReadOnlyList<PerceptionEntry>>>
        CreatePerceptionTrackerLazy(Guid entityId)
    {
        return new Lazy<PerceptionCollectionTrackerBase<List<PerceptionEntry>, IReadOnlyList<PerceptionEntry>>>(() => 
        {
            return new PerceptionListTracker(x => 
            {
                if (!x.TryGetEntityId(out var existingEntityId))
                {
                    return false;
                }
                return entityId == existingEntityId;
            });
        });
    }

    private static string GetPerceptionSourceKeyByEntityId(Guid entityId)
    {
        return $"entity-{entityId}";
    }
}
