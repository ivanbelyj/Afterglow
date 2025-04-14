using System.Linq;
using UnityEngine;

public record ThreatTargetingResult
{
    /// <summary>
    /// Ordered by utility descending
    /// </summary>
    public ThreatTargetingEstimate[] OrderedTargetingEstimates { get; set; }
    public ThreatTargetingEstimate TargetedThreat
        => OrderedTargetingEstimates.Any() ? OrderedTargetingEstimates.First() : null;
}
