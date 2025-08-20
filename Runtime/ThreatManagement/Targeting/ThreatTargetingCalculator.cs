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
        var (targeted, ignored) = SegregateEstimates(threats);
        return new()
        {
            OrderedTargetingEstimates = targeted
                .OrderByDescending(x => x.Utility)
                .ToArray(),
            Ignored = ignored
        };
    }

    private (List<ThreatTargetingEstimate> targeted, List<ThreatEstimate> ignored)
        SegregateEstimates(IEnumerable<ThreatEstimate> threats)
    {
        var targeted = new List<ThreatTargetingEstimate>();
        var ignored = new List<ThreatEstimate>();

        foreach (var threat in threats)
        {
            if (threat.Probability < parameters.threatProbabilityThreshold)
            {
                ignored.Add(threat);
                continue;
            }

            var maxPotentialUtility = GetMaxPotentialUtility(threat);

            if (maxPotentialUtility != null)
            {
                targeted.Add(new ThreatTargetingEstimate()
                {
                    ThreatEstimate = threat,
                    Utility = maxPotentialUtility.Value,
                });
            }
            else
            {
                // Ignore threats without potentials
                ignored.Add(threat);
            }

        }

        return (targeted, ignored);
    }

    private float? GetMaxPotentialUtility(ThreatEstimate threat)
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
        return maxPotentialUtility;
    }

    private float CalculateThreatUtility(
        ThreatEstimate threat,
        ThreatPotentialEstimate potential)
    {
        // TODO: take into account other factors
        return threat.Probability;
    }
}
