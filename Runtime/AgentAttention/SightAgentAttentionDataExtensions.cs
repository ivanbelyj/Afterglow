using System;
using System.Collections.Generic;
using UnityEngine;

public static class SightAgentAttentionDataExtensions
{
    public static bool TryGetAgentAttentionData(
        this Sight sight,
        out List<AgentAttentionData> entityAttentionData)
        => sight.TryGet(SightDataKeys.AgentAttention, out entityAttentionData);

    public static void SetAgentAttentionData(
        this Sight sight,
        List<AgentAttentionData> entityAttentionData)
        => sight.Set(SightDataKeys.AgentAttention, entityAttentionData);
}
