using UnityEngine;

public struct ThreatFactorPotential
{
    /// <summary>
    /// The radius within which an entity can deal damage
    /// </summary>
    public BoundCompoundProperty Radius { get; set; }

    /// <summary>
    /// The degree of the threat's ability to cause harm. Unlimited positive value
    /// </summary>
    public BoundCompoundProperty Potential { get; set; }

    /// <summary>
    /// Estimated time until the threat becomes active (seconds).
    /// 0 - already can attack
    /// </summary>
    public BoundCompoundProperty ActivationTimeSeconds { get; set; }
}
