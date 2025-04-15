using System;
using System.Collections.Generic;
using UnityEngine;

using static ThreatPerceptionEntryDataKeys;

public static class ThreatPerceptionEntryExtensions
{
    public static bool CanBeThreat(this PerceptionEntry perception)
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
}
