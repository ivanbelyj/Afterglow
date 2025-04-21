using System;
using System.Collections.Generic;
using UnityEngine;

using static PerceptionEntryCoreDataKeys;

public static class PerceptionEntryCoreExtensions
{
    public static bool TryGetEntityType(
        this PerceptionEntry perceptionEntry,
        out string entityType)
        => perceptionEntry.TryGet(EntityType, out entityType);

    public static bool TryGetEntityId(
        this PerceptionEntry perceptionEntry,
        out Guid entityId)
        => perceptionEntry.TryGet(EntityId, out entityId);

    public static bool TryGetPosition(
        this PerceptionEntry perception,
        out SpatialAwarenessPosition position)
        => perception.TryGet(Position, out position);

    public static bool TryGetConstructKey(
        this PerceptionEntry perception,
        out string constructKey)
        => perception.TryGet(Construct, out constructKey);

    public static bool HasEntityId(this PerceptionEntry perception)
        => perception.HasKey(EntityId);
    public static bool HasPosition(this PerceptionEntry perception)
        => perception.HasKey(Position);
}
