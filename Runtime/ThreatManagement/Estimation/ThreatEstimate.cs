using System;
using UnityEngine;

public record ThreatEstimate
{
    public Guid EntityId { get; set; }
    public string ThreatType { get; set; }
    public float Distance { get; set; }
    public ThreatPotentialEstimate[] Potentials { get; set; }

    /// <summary>
    /// Probability of a combat encounter with a threat when one of potentials
    /// is reached. 1 = already fighting
    /// </summary>
    public float Probability { get; set; }
    
    public IThreatPerception ThreatPerception { get; }

    public ThreatEstimate(IThreatPerception threatPerception)
    {
        ThreatPerception = threatPerception;
    }

    public override string ToString()
    {
        var potentialsString = Potentials != null 
            ? string.Join(", ", Array.ConvertAll(Potentials, p => p.ToString())) 
            : "None";
        
        return $"ThreatEstimate: {ThreatType} (Entity: {EntityId}, Probability: {Probability:P0})\n" +
               $"Potentials: [{potentialsString}]";
    }
}
