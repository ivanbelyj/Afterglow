using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public record ThreatTargetingResult
{
    /// <summary>
    /// Ordered by utility descending
    /// </summary>
    public IReadOnlyList<ThreatTargetingEstimate> OrderedTargetingEstimates { get; set; }
    public ThreatTargetingEstimate TargetedThreat
        => OrderedTargetingEstimates.Any() ? OrderedTargetingEstimates.First() : null;
    public IReadOnlyList<ThreatEstimate> Ignored { get; set; }
}
