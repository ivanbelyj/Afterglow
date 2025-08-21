using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// The agent has cumulative dynamic understanding of other entities
/// that have been encountered in the game.
/// </summary>
public class EntityConstructPerceptionManager :
    ConstructPerceptionManagerBase<EntityConstructPerceptionManager.EntityConstructArgs>
{
    public struct EntityConstructArgs
    {
        public Guid EntityId { get; set; }
        public string EntityType { get; set; }
    }

    // Perception data keys and markers
    public const string MaxMovementSpeed = nameof(MaxMovementSpeed);
    public const string MinMovementSpeed = nameof(MinMovementSpeed);
    public const string EntityConstruct = nameof(EntityConstruct);

    private static readonly ConstructPerceptionConfig aggregateConfig;

    static EntityConstructPerceptionManager()
    {
        var builder = new ConstructPerceptionConfigBuilder();

        builder
            .AggregateMax<float>(MaxMovementSpeed, ThreatPerceptionEntryDataKeys.MovementSpeed)
            // Todo: temporary config
            .AggregateSum<float>("TotalPain", "Pain")
            .AggregateCount("PainCount", "Pain");

        aggregateConfig = builder.Build();
    }

    public PerceptionEntry TouchEntityConstruct(Guid entityId, string entityType)
    {
        return EnsureConstructPushed(new()
        {
            EntityId = entityId,
            EntityType = entityType
        }).construct;
    }

    public PerceptionEntry TouchEntityConstruct(
        Guid entityId,
        string entityType,
        IEnumerable<PerceptionEntry> newPerceptions)
    {
        var (constructKey, construct) = EnsureConstructPushed(new()
        {
            EntityId = entityId,
            EntityType = entityType
        });

        perceptionConstructGate.UpdateConstruct(constructKey, newPerceptions);

        return construct;
    }

    public static float? GetMaxMovementSpeed(PerceptionEntry perception)
        => GetPerceptionValue(perception, MaxMovementSpeed);

    protected override string GetConstructKey(EntityConstructArgs constructArgs)
    {
        return $"entity-construct-{constructArgs.EntityId}";
    }

    protected override bool ShouldBeTrackedByConstruct(
        EntityConstructArgs constructArgs,
        PerceptionEntry construct,
        PerceptionEntry newPerception)
        => (newPerception.TryGetEntityId(out var newPerceptionEntityId)
                && newPerceptionEntityId == constructArgs.EntityId
            || newPerception.TryGet<Guid>(
                PerceptionEntryCoreDataKeys.CauseEntityId,
                out var newPerceptionCauseEntityId)
                && newPerceptionCauseEntityId == constructArgs.EntityId)
            // Don't track other constructs
            && !newPerception.Markers.Contains(EntityConstruct);

    protected override void HandleConstruct(
        PerceptionEntry construct,
        PerceptionEntry newPerception)
    {
        aggregateConfig.ApplyToConstruct(construct, newPerception);

        const int MinPainCountForAverage = 4;

        if (construct.TryGet<float>("TotalPain", out var totalPain) &&
            construct.TryGet<int>("PainCount", out var painCount) &&
            painCount >= MinPainCountForAverage)
        {
            construct.Set("AveragePain", totalPain / painCount);
        }
    }

    protected override void HandleConstructBeforePush(
        EntityConstructArgs constructArgs,
        PerceptionEntry constructPerception)
    {
        // Add markers and perception data
        constructPerception.Markers.Add(EntityConstruct);
        constructPerception.Set(
            PerceptionEntryCoreDataKeys.EntityId,
            constructArgs.EntityId);
        constructPerception.Set(
            PerceptionEntryCoreDataKeys.EntityType,
            constructArgs.EntityType);
    }

    protected override IEnumerable<ConstructPerceptionConfig> GetConfiguration()
    {
        yield return aggregateConfig;
    }
}
