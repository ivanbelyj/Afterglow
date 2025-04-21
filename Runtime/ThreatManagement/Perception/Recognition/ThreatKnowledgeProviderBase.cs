using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using static PerceptionMarkerUtils;

[RequireComponent(typeof(EntityProvider))]
public abstract class ThreatKnowledgeProviderBase : MonoBehaviour, IThreatKnowledgeProvider
{
    // private const string MovementSpeedStatisticName = "MovementSpeed";

    [SerializeField, Required]
    private PerceptionStorageRegistrar perceptionStorageRegistrar;

    [SerializeField, Required]
    private EntityConstructPerceptionManager entityConstructManager;

    private ISegregatedMemoryProvider segregatedMemoryProvider;
    private EntityProvider entityProvider;

    // private ThreatKnowledgeStatisticsHelper statisticsHelper = new();

    private void Awake()
    {
        segregatedMemoryProvider = GetSegregatedMemoryProvider();
        entityProvider = GetComponent<EntityProvider>();

        if (perceptionStorageRegistrar == null)
        {
            Debug.LogError($"{nameof(perceptionStorageRegistrar)} is required");
        }

        // statisticsHelper.InitializeStatistics(
        //     new List<PerceptionEntry>(),
        //     new ThreatKnowledgeStatisticsHelper.ThreatStatistic(
        //         MovementSpeedStatisticName,
        //         perception =>
        //         {
        //             var hasMovementSpeed = perception.TryGetMovementSpeed(out var movementSpeed);
        //             return (hasMovementSpeed, movementSpeed);
        //         }));
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
            if (!perception.TryGetEntityType(out var entityType))
            {
                Debug.LogError(
                    $"Perception with entity id set must also contain entity type");
            }

            var factor = GetEntityIndividualFactor(entityId, entityType);
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

    private ThreatPerceptionCompoundData GetEntityIndividualFactor(
        Guid entityId,
        string entityType)
    {
        // All perceptions including past
        var perceptions = GetPerceptionsByEntity(entityId);

        if (!perceptions.Any())
        {
            return null;
        }

        var actualPerceptions = GetActualPerceptions(perceptions);

        var (threatAwareness, threatFocus, actualMovementSpeed) = GetActualThreatData(
            entityProvider.Entity.Id,
            actualPerceptions
        );

        // Update perception construct explicitly and get statistics
        var construct = entityConstructManager.TouchEntityConstruct(
            entityId,
            entityType,
            actualPerceptions);
        var maxStatisticMovementSpeed = EntityConstructPerceptionManager
            .GetMaxMovementSpeed(construct) ?? 0;

        var optimisticMovementSpeed = Math.Min(actualMovementSpeed, maxStatisticMovementSpeed);
        var pessimisticMovementSpeed = Math.Max(actualMovementSpeed, maxStatisticMovementSpeed);

        return new ThreatPerceptionCompoundData()
        {
            MovementSpeed = new(optimisticMovementSpeed, pessimisticMovementSpeed),
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

    private (float threatAwareness, float threatFocus, float movementSpeed) GetActualThreatData(
        Guid targetEntityId,
        List<PerceptionEntry> actualPerceptions)
    {
        float? maxThreatAwareness = null;
        float? maxThreatFocus = null;
        float? maxMovementSpeed = null;

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

            bool hasMovementSpeed = perception.TryGetMovementSpeed(out var movementSpeed);

            if (hasMovementSpeed
                && (maxMovementSpeed == null || movementSpeed > maxMovementSpeed))
            {
                maxMovementSpeed = movementSpeed;
            }
        }

        return (maxThreatAwareness ?? 0, maxThreatFocus ?? 0, maxMovementSpeed ?? 0);
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
        return new()
        {
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
        return new ThreatPerceptionCompoundData()
        {
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
