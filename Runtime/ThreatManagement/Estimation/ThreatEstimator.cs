using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ThreatEstimator
{
    private readonly PerceptionStorageRegistrar perceptionStorageRegistrar;

    public ThreatEstimator(PerceptionStorageRegistrar perceptionStorageRegistrar)
    {
        this.perceptionStorageRegistrar = perceptionStorageRegistrar;
    }

    public IEnumerable<ThreatEstimate> Estimate(
        Guid estimateOriginEntityId,
        SpatialAwarenessPosition estimateOriginPosition,
        IEnumerable<IThreatPerception> perceptions)
    {
        return perceptions.Select(perception => EstimatePerception(estimateOriginEntityId, estimateOriginPosition, perception));
    }

    private ThreatEstimate EstimatePerception(
        Guid estimateOriginEntityId,
        SpatialAwarenessPosition estimateOriginPosition,
        IThreatPerception threatPerception)
    {
        var data = threatPerception.ThreatKnowledge.GetCompoundData();
        var probabilityProperty = data.ThreatPresence
            * data.PerceptorSuspicion
            * data.ThreatAwareness
            * data.ThreatFocus;
        var threatPosition = threatPerception.Position;
        return new(threatPerception) 
        {
            EntityId = threatPerception.EntityId,
            ThreatType = EntityTypeToThreatType(threatPerception.EntityType),
            Distance = GetDistance(
                estimateOriginPosition.Position,
                estimateOriginPosition.Radius,
                threatPosition.Position,
                threatPosition.Radius),
            Potentials = data.Potentials.Select(x => GetRadiusPotentialEstimate(
                estimateOriginPosition,
                threatPosition.Position,
                x,
                data.MovementSpeed)).ToArray(),
            Probability = (float)probabilityProperty
        };
    }

    private List<PerceptionEntry> GetPerceptionsByEntity(Guid entityId)
    {
        return perceptionStorageRegistrar.GetPerceptionsFromRegisteredStorageForEntity(entityId);
    }

    private string EntityTypeToThreatType(string entityType)
    {
        // Similar for now
        return entityType;
    }

    private ThreatPotentialEstimate GetRadiusPotentialEstimate(
        SpatialAwarenessPosition perceptorPosition,
        Vector3 threatPosition,
        ThreatFactorPotential radiusThreat,
        BoundCompoundProperty movementSpeed)
    {
        var distance = GetDistance(
            perceptorPosition.Position,
            perceptorPosition.Radius,
            threatPosition,
            (float)radiusThreat.Radius
        );

        // If the speed is zero, the threat will never reach the radius.
        if ((float)movementSpeed <= Mathf.Epsilon)
        {
            return CreateResult(distance <= Mathf.Epsilon ? 0f : null);
        }
        
        float movementTime = distance / movementSpeed.GetValue();
        
        return CreateResult(movementTime + (float)radiusThreat.ActivationTimeSeconds);

        ThreatPotentialEstimate CreateResult(float? timeDistance)
            => new () {
                Potential = (float)radiusThreat.Potential,
                PotentialAvailableInSeconds = timeDistance
            };
    }

    private float GetDistance(
        Vector3 position1,
        float radius1,
        Vector3 position2,
        float radius2)
    {
        // TODO: take accessibility into account
        return Mathf.Max(
            0,
            Vector3.Distance(position1, position2) - radius1 - radius2);
    }
}
