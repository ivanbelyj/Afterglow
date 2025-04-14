using UnityEngine;

public struct ThreatPotentialEstimate
{
    public float? PotentialAvailableInSeconds { get; set; }
    public float Potential { get; set; }

    public override string ToString()
    {
        return PotentialAvailableInSeconds.HasValue 
            ? $"Potential: {Potential} (Available in {PotentialAvailableInSeconds}s)"
            : $"Potential: {Potential} (Not available)";
    }
}
