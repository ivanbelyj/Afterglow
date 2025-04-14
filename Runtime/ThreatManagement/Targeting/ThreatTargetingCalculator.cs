using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ThreatTargetingCalculator
{
    private readonly ThreatTargetingParameters parameters;
    
    public ThreatTargetingCalculator(ThreatTargetingParameters parameters)
    {
        this.parameters = parameters;
    }

    public ThreatTargetingResult CalculateTargeting(IEnumerable<ThreatEstimate> threats)
    {
        return new()
        {
            OrderedTargetingEstimates = GetTargetingEstimates(threats)
                .OrderByDescending(x => x.Utility)
                .ToArray(),
        };
    }

    private IEnumerable<ThreatTargetingEstimate> GetTargetingEstimates(IEnumerable<ThreatEstimate> threats)
    {
        foreach (var threat in threats.Where(x => x.Probability >= parameters.threatProbabilityThreshold))
        {
            float? maxPotentialUtility = null;
            foreach (var threatPotential in threat.Potentials)
            {
                var utility = CalculateThreatUtility(threat, threatPotential);

                if (maxPotentialUtility == null || utility > maxPotentialUtility)
                {
                    maxPotentialUtility = utility;
                }
            }
            if (maxPotentialUtility != null)
            {
                yield return new ThreatTargetingEstimate()
                {
                    ThreatEstimate = threat,
                    Utility = maxPotentialUtility.Value,
                };
            }
        }
    }

    private float CalculateThreatUtility(
        ThreatEstimate threat,
        ThreatPotentialEstimate potential)
    {
        // TODO: take into account other factors
        return threat.Probability;
    }
}
