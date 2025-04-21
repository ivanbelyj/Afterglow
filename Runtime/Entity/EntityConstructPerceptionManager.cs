using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The agent has cumulative dynamic understanding of other entities
/// that he has encountered in the game.
/// </summary>
public class EntityConstructPerceptionManager : BaseConstructPerceptionManager
{
    // Perception data keys and markers
    public const string MaxMovementSpeedDataKey = nameof(MaxMovementSpeedDataKey);
    public const string EntityConstructMarker = nameof(EntityConstructMarker);

    private static readonly ConstructPerceptionAggregationConfig<float>[] aggregateConfigs = new[]
    {
        ConstructPerceptionAggregationConfig<float>.AggregateMax(
            MaxMovementSpeedDataKey,
            ThreatPerceptionEntryDataKeys.MovementSpeed
        )
    };

    [SerializeField, Required]
    private PerceptionConstructGate perceptionConstructGate;

    public PerceptionEntry TouchEntityConstruct(Guid entityId, string entityType)
    {
        return EnsurePushed(entityId, entityType).construct;
    }

    public PerceptionEntry TouchEntityConstruct(
        Guid entityId,
        string entityType,
        IEnumerable<PerceptionEntry> newPerceptions)
    {
        var (constructKey, construct) = EnsurePushed(entityId, entityType);

        perceptionConstructGate.UpdateConstruct(constructKey, newPerceptions);

        return construct;
    }

    public static float? GetMaxMovementSpeed(PerceptionEntry perception)
        => GetPerceptionValue(perception, MaxMovementSpeedDataKey);

    public static string GetEntityConstructKey(Guid entityId)
    {
        return $"entity-construct-{entityId}";
    }

    private (string constructKey, PerceptionEntry construct) EnsurePushed(
        Guid entityId,
        string entityType)
    {
        var constructKey = GetEntityConstructKey(entityId);
        if (!perceptionConstructGate.TryGetConstruct(
            constructKey,
            out var constructPerception))
        {
            constructPerception = perceptionConstructGate.PushConstruct(
                constructKey,
                (construct, newPerception)
                    => newPerception.TryGetEntityId(out var _)
                        // Don't track other constructs
                        && !newPerception.Markers.Contains(EntityConstructMarker),
                HandleEntityConstruct,
                constructPerception =>
                {
                    // Add markers and perception data
                    constructPerception.Markers.Add(EntityConstructMarker);
                    constructPerception.Set(PerceptionEntryCoreDataKeys.EntityId, entityId);
                    constructPerception.Set(PerceptionEntryCoreDataKeys.EntityType, entityType);
                });
        }

        return (constructKey, constructPerception);
    }

    private void HandleEntityConstruct(
        PerceptionEntry construct,
        PerceptionEntry newPerception)
    {
        HandleConstruct(construct, newPerception, aggregateConfigs);
    }
}
