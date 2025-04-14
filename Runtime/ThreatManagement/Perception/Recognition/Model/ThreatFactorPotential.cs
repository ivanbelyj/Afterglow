using UnityEngine;

public record ThreatFactorPotential
{
    /// <summary>
    /// For example: 'close-combat', 'pistol', 'shotgun', etc.
    /// </summary>
    public string PotentialName { get; set; }

    /// <summary>
    /// The radius within which an entity can deal damage
    /// </summary>
    public BoundCompoundProperty Radius { get; set; }

    /// <summary>
    /// The degree of the threat's ability to cause harm.
    /// </summary>
    public BoundCompoundProperty Potential { get; set; }

    /// <summary>
    /// Estimated time until the threat becomes active (seconds).
    /// 0 - already can attack
    /// </summary>
    public BoundCompoundProperty ActivationTimeSeconds { get; set; }

    public ThreatFactorPotential(string potentialName)
    {
        PotentialName = potentialName;
    }
}
