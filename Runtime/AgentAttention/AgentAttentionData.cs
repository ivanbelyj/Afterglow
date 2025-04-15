using System;
using UnityEngine;

/// <summary>
/// Represents agent's attention focus on a specific target,
/// including awareness and engagement metrics.
/// This data structure is used to track how much attention an entity
/// is giving to another entity in the environment
/// </summary
public record AgentAttentionData
{
    /// <summary>
    /// The unique identifier of the target entity being observed.
    /// </summary>
    public Guid TargetEntityId { get; set; }

    /// <summary>
    /// Threat's awareness of the target entity.
    /// 0 = hasn't noticed, 1 = fully aware.
    /// </summary>
    public float ThreatAwareness { get; set; }

    /// <summary>
    /// Threat's activity or interest towards the target entity.
    /// 0 = degree of interest is unknown or the entity is inactive towards
    /// the target currently (focused on another opponent, for example);
    /// 1 = the entity is fully focused on the target
    /// </summary>
    public float ThreatFocus { get; set; }

    public override string ToString()
    {
        return $"Attention on {TargetEntityId}: " +
            $"Awareness={ThreatAwareness:F2}, " +
            $"Focus={ThreatFocus:F2}";
    }
}
