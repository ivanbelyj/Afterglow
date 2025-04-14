using System;
using System.Collections.Generic;
using UnityEngine;

public static class SightAgentAttentionDataExtensions
{
    public static bool TryGetAgentAttentionData(
        this Sight sight,
        out List<AgentAttentionData> entityAttentionData)
        => sight.TryGetList(SightDataKeys.AgentAttention, out entityAttentionData);

    public static void AddAgentAttentionData(
        this Sight sight,
        AgentAttentionData entityAttentionData)
        => sight.AddToList(SightDataKeys.AgentAttention, entityAttentionData);
}
