using System;
using System.Collections.Generic;
using UnityEngine;

using static ThreatPerceptionEntryDataKeys;

public static class ThreatPerceptionEntryExtensions
{
    public static bool SatisfiesThreatNecessaryCondition(this PerceptionEntry perception)
    {
        return perception.HasEntityId()
            && perception.TryGetEntityType(out _)
            && perception.HasPosition();
    }

    public static bool TryGetEntitySuspicion(
        this PerceptionEntry perceptionEntry,
        out float entitySuspicion)
        => perceptionEntry.TryGet(EntitySuspicion, out entitySuspicion);

    public static bool TryGetAgentAttentionData(
        this PerceptionEntry perceptionEntry,
        out List<AgentAttentionData> entityAttentionData)
        => perceptionEntry.TryGet(AgentAttention, out entityAttentionData);

    public static void SetAgentAttentionData(
        this PerceptionEntry perceptionEntry,
        List<AgentAttentionData> entityAttentionData)
        => perceptionEntry.Set(AgentAttention, entityAttentionData);

    public static bool TryGetMovementSpeed(
        this PerceptionEntry perceptionEntry,
        out float movementSpeed)
        => perceptionEntry.TryGet(MovementSpeed, out movementSpeed);

    public static void SetMovementSpeed(
        this PerceptionEntry perceptionEntry,
        float movementSpeed)
        => perceptionEntry.Set(MovementSpeed, movementSpeed);
}
