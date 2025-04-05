using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ThreatEstimator
{
    public IEnumerable<ThreatEstimate> Estimate(IEnumerable<IThreatPerception> perceptions)
    {
        return perceptions.Select(EstimatePerception);
    }

    private ThreatEstimate EstimatePerception(IThreatPerception threatPerception)
    {
        var data = threatPerception.ThreatKnowledge.GetCompoundData();
        var probabilityProperty = data.ThreatPresence
            * data.PerceptorSuspicion
            * data.ThreatAwareness
            * data.ThreatFocus;
        return new() 
        {
            Potentials = data.Potentials.Select(x => GetRadiusPotentialEstimate(
                threatPerception.Position.Position,
                x,
                data.MovementSpeed)).ToArray(),
            Probability = (float)probabilityProperty
        };
    }

    private ThreatPotentialEstimate GetRadiusPotentialEstimate(
        Vector3 threatPosition,
        ThreatFactorPotential radiusThreat,
        BoundCompoundProperty movementSpeed)
    {
        // If the speed is zero, the threat will never reach the radius.
        if ((float)movementSpeed <= Mathf.Epsilon)
        {
            return CreateResult(null);
        }
        
        float movementTime = GetDistanceToRadius(threatPosition, (float)radiusThreat.Radius)
            / movementSpeed.GetValue();
        
        return CreateResult(movementTime + (float)radiusThreat.ActivationTimeSeconds);

        ThreatPotentialEstimate CreateResult(float? timeDistance)
            => new () {
                Potential = (float)radiusThreat.Potential,
                PotentialAvailableInSeconds = timeDistance
            };
    }

    private float GetDistanceToRadius(Vector3 threatPosition, float radius)
    {
        // TODO: take accessibility into account
        return Mathf.Max(0, Vector3.Distance(threatPosition, Vector3.zero) - radius);
    }
}
