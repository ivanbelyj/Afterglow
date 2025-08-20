using UnityEngine;

[System.Serializable]
public record ThreatFactorPotential
{
    [SerializeField]
    private string potentialName;
    [SerializeField]
    private BoundCompoundProperty radius;
    [SerializeField]
    private BoundCompoundProperty potential;
    [SerializeField]
    private BoundCompoundProperty activationTimeSeconds;

    /// <summary>
    /// For example: 'close-combat', 'pistol', 'shotgun', etc.
    /// </summary>
    public string PotentialName
    {
        get => potentialName; set => potentialName = value;
    }

    /// <summary>
    /// The radius within which an entity can deal damage
    /// </summary>
    public BoundCompoundProperty Radius
    {
        get => radius; set => radius = value;
    }

    /// <summary>
    /// The degree of the threat's ability to cause harm.
    /// </summary>
    public BoundCompoundProperty Potential
    {
        get => potential; set => potential = value;
    }

    /// <summary>
    /// Estimated time until the threat becomes active (seconds).
    /// 0 - already can attack
    /// </summary>
    public BoundCompoundProperty ActivationTimeSeconds
    {
        get => activationTimeSeconds; set => activationTimeSeconds = value;
    }

    public ThreatFactorPotential(string potentialName)
    {
        PotentialName = potentialName;
    }
}
