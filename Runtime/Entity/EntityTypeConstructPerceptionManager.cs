using System;
using System.Collections.Generic;
using UnityEngine;

public class EntityTypeConstructPerceptionManager : BaseConstructPerceptionManager
{
    public const string MinTypicalMovementSpeed = nameof(MinTypicalMovementSpeed);
    public const string MaxTypicalMovementSpeed = nameof(MaxTypicalMovementSpeed);
    public const string MinTypicalSuspicion = nameof(MinTypicalSuspicion);
    public const string MaxTypicalSuspicion = nameof(MaxTypicalSuspicion);

    private static readonly ConstructPerceptionAggregationConfig<float>[] aggregateConfigs = new[]
    {
        // TODO: we need average by entities MaxMovementSpeed
        ConstructPerceptionAggregationConfig<float>.AggregateMax(
            MaxTypicalMovementSpeed,
            EntityConstructPerceptionManager.MaxMovementSpeedDataKey
        )
    };

    [SerializeField, Required]
    private PerceptionConstructGate perceptionConstructGate;

    private void Start()
    {
        // TODO: for debug now
        TouchEntityTypeConstruct("Avrian");
    }

    public PerceptionEntry TouchEntityTypeConstruct(string entityType)
    {
        return EnsurePushed(entityType);
    }

    public static string GetEntityTypeConstructKey(string entityType)
    {
        return $"entity-type-construct-{entityType}";
    }

    private PerceptionEntry EnsurePushed(string entityType)
    {
        return perceptionConstructGate.EnsureConstructPushed(
            GetEntityTypeConstructKey(entityType),
            (construct, newPerception)
                => ShouldBeTrackedByConstruct(entityType, construct, newPerception),
            HandleEntityTypeConstruct,
            null);
    }

    private bool ShouldBeTrackedByConstruct(
        string entityType,
        PerceptionEntry construct,
        PerceptionEntry newPerception)
    {
        // We are interested in entity construct perceptions of our type
        return newPerception.Markers.Contains(
                EntityConstructPerceptionManager.EntityConstructMarker)
            && newPerception.TryGetEntityType(out var newPerceptionEntityType)
            && newPerceptionEntityType == entityType;
    }

    private void HandleEntityTypeConstruct(
        PerceptionEntry hyperConstruct,
        PerceptionEntry relatedConstruct)
    {
        HandleConstruct(hyperConstruct, relatedConstruct, aggregateConfigs);
    }
}
