using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using static PerceptionMarkerUtilsCore;

[RequireComponent(typeof(EntityProvider))]
public abstract class ThreatKnowledgeProviderBase : MonoBehaviour, IThreatKnowledgeProvider
{
    [SerializeField]
    private PerceptionStorageRegistrar perceptionStorageRegistrar;

    private ISegregatedMemoryProvider segregatedMemoryProvider;
    private EntityProvider entityProvider;
    
    private void Awake()
    {
        segregatedMemoryProvider = GetSegregatedMemoryProvider();
        entityProvider = GetComponent<EntityProvider>();

        if (perceptionStorageRegistrar == null)
        {
            Debug.LogError($"{nameof(perceptionStorageRegistrar)} is required");
        }
    }

    public ThreatKnowledge WhatAboutThreat(PerceptionEntry perceptionEntry)
    {
        var factors = new List<ThreatPerceptionCompoundData>();

        var entityType = TryAddEntityTypeFactor(perceptionEntry, factors);
        if (entityType == null)
        {
            // Consider as not a threat (but it's not correct case, maybe)
            return null;
        }

        TryAddEntityFactor(perceptionEntry, factors);     

        // No factors - no threat
        if (!factors.Any())
        {
            return null;
        }

        return new ThreatKnowledge()
        {
            factors = factors,
            threatType = entityType,
        };
    }

    // Movement speed is typically obvious, so it's considered always known.
    // Should be implemented at the level of a specific project.
    protected abstract ThreatEntityTypeKnowledge GetEntityTypeKnowledge(string entityType);

    protected virtual ISegregatedMemoryProvider GetSegregatedMemoryProvider()
    {
        return GetComponent<SegregatedMemoryManager>();
    }

    private void TryAddEntityFactor(
        PerceptionEntry perception,
        List<ThreatPerceptionCompoundData> factors)
    {
        bool hasEntityId = perception.TryGetEntityId(out var entityId);

        if (hasEntityId)
        {
            var factor = GetEntityIndividualFactor(entityId);
            if (factor != null)
            {
                factors.Add(factor);
            }
        }
    }

    /// <returns>Entity type</returns>
    private string TryAddEntityTypeFactor(
        PerceptionEntry perception,
        List<ThreatPerceptionCompoundData> factors)
    {
        bool hasEntityType = perception.TryGetEntityType(out var entityType);

        if (hasEntityType)
        {
            var entityTypeFactor = GetEntityTypeFactor(entityType);
            if (entityTypeFactor != null)
            {
                factors.Add(entityTypeFactor);
                return entityType;
            }
        }
        return null;
    }

    private List<PerceptionEntry> GetPerceptionsByEntity(Guid entityId)
    {
        return perceptionStorageRegistrar.GetPerceptionsFromRegisteredStorageForEntity(entityId);
    }

    private ThreatPerceptionCompoundData GetEntityIndividualFactor(Guid entityId)
    {
        // All perceptions including past
        var perceptions = GetPerceptionsByEntity(entityId);

        if (!perceptions.Any())
        {
            return null;
        }

        var actualPerceptions = GetActualPerceptions(perceptions);

        var (threatAwareness, threatFocus) = GetThreatAttentionData(
            entityProvider.Entity.Id,
            actualPerceptions
        );

        return new ThreatPerceptionCompoundData() {
            PerceptorSuspicion = GetEntitySuspicion(perceptions),
            Potentials = GetEntityIndividualPotentials(actualPerceptions),

            ThreatAwareness = new(threatAwareness, threatAwareness),
            ThreatFocus = new(threatFocus, threatFocus),
            ThreatPresence = GetThreatPresence(entityId, actualPerceptions),
        };
    }

    private BoundCompoundProperty GetThreatPresence(
        Guid entityId,
        List<PerceptionEntry> actualPerceptions)
    {
        bool isVisible = IsVisibleCurrently(entityId);
        float minPresence = isVisible ? 1f : 0;
        float maxPresence = isVisible ? 1f : 0;
        
        return new(minPresence, maxPresence);
    }

    private bool IsVisibleCurrently(Guid entityId)
    {
        return segregatedMemoryProvider
            .GetPerceptions(
                CoreSegregatedPerceptionSources.VisionSensoryMemory,
                GetEntityUniqueMarker(entityId))
            .Any();
    }

    private (float threatAwareness, float threatFocus) GetThreatAttentionData(
        Guid targetEntityId,
        List<PerceptionEntry> actualPerceptions)
    {
        float? maxThreatAwareness = null;
        float? maxThreatFocus = null;

        foreach (var perception in actualPerceptions)
        {
            var attentionData = GetRelevantAttentionData(perception, targetEntityId);
            
            if (attentionData != null)
            {
                if (maxThreatAwareness == null
                    || attentionData.ThreatAwareness > maxThreatAwareness)
                {
                    maxThreatAwareness = attentionData.ThreatAwareness;
                }

                if (maxThreatFocus == null
                    || attentionData.ThreatFocus > maxThreatFocus)
                {
                    maxThreatFocus = attentionData.ThreatFocus;
                }
            }
        }

        return (maxThreatAwareness ?? 0, maxThreatFocus ?? 0);
    }

    private AgentAttentionData GetRelevantAttentionData(
        PerceptionEntry perception,
        Guid targetEntityId)
    {
        if (!perception.TryGetAgentAttentionData(
            out var allAttentionData))
        {
            return null;
        }
        
        return allAttentionData.Find(x => x.TargetEntityId == targetEntityId);
    }

    private List<PerceptionEntry> GetActualPerceptions(List<PerceptionEntry> perceptions)
    {
        return new List<PerceptionEntry> {
            perceptions.MaxBy(x => x.TimestampTo ?? x.TimestampFrom ?? float.MinValue)
        };
    }

    private List<ThreatFactorPotential> GetEntityIndividualPotentials(
        List<PerceptionEntry> perceptionsByEntity)
    {
        return new() { 
            // Don't consider any individual or dynamic potentials for now.
            // There can be potentials built by seen weapons or learnt
            // threat's individual characteristics
        };
    }

    private BoundCompoundProperty GetEntitySuspicion(List<PerceptionEntry> perceptions)
    {
        float? minSuspicion = null;
        float? maxSuspicion = null;
        
        foreach (var perception in perceptions)
        {
            if (perception.TryGetEntitySuspicion(out var entitySuspicion))
            {
                if (minSuspicion == null || entitySuspicion < minSuspicion)
                {
                    minSuspicion = entitySuspicion;
                }

                if (maxSuspicion == null || entitySuspicion > maxSuspicion)
                {
                    maxSuspicion = entitySuspicion;
                }
            }
        }

        return new(minSuspicion ?? 0, maxSuspicion ?? 0);
    }

    private ThreatPerceptionCompoundData GetEntityTypeFactor(string entityType)
    {
        var entityTypeKnowledge = GetEntityTypeKnowledge(entityType);
        if (entityTypeKnowledge == null)
        {
            return null;
        }
        return new ThreatPerceptionCompoundData() {
            MovementSpeed = entityTypeKnowledge.MovementSpeed,
            PerceptorSuspicion = entityTypeKnowledge.TypicalSuspicion,
            Potentials = entityTypeKnowledge.BasePotentials,
        };
    }
}

public class ThreatEntityTypeKnowledge
{
    public BoundCompoundProperty MovementSpeed { get; set; }
    public BoundCompoundProperty TypicalSuspicion { get; set; }
    public List<ThreatFactorPotential> BasePotentials { get; set; }
}
