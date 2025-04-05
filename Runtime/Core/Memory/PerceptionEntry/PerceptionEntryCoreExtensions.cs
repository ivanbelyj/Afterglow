using System;
using System.Collections.Generic;
using UnityEngine;

using static PerceptionEntryCoreDataKeys;

public static class PerceptionEntryCoreExtensions
{
    public static bool HasKey(this PerceptionEntry perception, string key)
    {
        return perception.PerceptionData.ContainsKey(key);
    }

    public static T GetValue<T>(this PerceptionEntry perception, string key)
    {
        if (!perception.HasKey(key))
            throw new KeyNotFoundException($"Key {key} not found in perception data");

        return (T)perception.PerceptionData[key];
    }
    
    public static bool HasEntityId(this PerceptionEntry perception) => perception.HasKey(EntityId);
    public static bool HasPosition(this PerceptionEntry perception) => perception.HasKey(Position);
    public static Guid GetEntityId(this PerceptionEntry perception) => perception.GetValue<Guid>(EntityId);
    public static SpatialAwarenessPosition GetPosition(this PerceptionEntry perception) => perception.GetValue<SpatialAwarenessPosition>(Position);
}
