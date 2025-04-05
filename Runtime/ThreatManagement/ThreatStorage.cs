using System;
using System.Collections.Generic;
using UnityEngine;

public class ThreatStorage : MonoBehaviour, IThreatPerceptionProvider
{
    private Dictionary<Guid, IThreatPerception> threatPerceptionsByEntityId = new();

    public IEnumerable<IThreatPerception> GetActualThreats()
    {
        Debug.Log("Current treats: " + threatPerceptionsByEntityId.Count);
        return threatPerceptionsByEntityId.Values;
    }

    public void CaptureThreat(IThreatPerception threatPerception)
    {
        if (threatPerception == null)
        {
            throw new ArgumentNullException(nameof(threatPerception));
        }

        if (!threatPerceptionsByEntityId.TryGetValue(threatPerception.EntityId, out var existingThreat))
        {
            threatPerceptionsByEntityId[threatPerception.EntityId] = threatPerception;
            return;
        }

        if (ShouldReplaceExistingThreat(existingThreat, threatPerception))
        {
            threatPerceptionsByEntityId[threatPerception.EntityId] = threatPerception;
        }
    }

    private bool ShouldReplaceExistingThreat(IThreatPerception existing, IThreatPerception newThreat)
    {
        if (!newThreat.Timestamp.HasValue)
        {
            return false;
        }
            

        if (!existing.Timestamp.HasValue)
        {
            return true;
        }

        return newThreat.Timestamp.Value >= existing.Timestamp.Value;
    }
}
