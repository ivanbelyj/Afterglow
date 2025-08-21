using System;
using System.Collections.Generic;
using UnityEngine;

public class EntityTypeConstructPerceptionManager :
    ConstructPerceptionManagerBase<EntityTypeConstructPerceptionManager.EntityTypeConstructArgs>
{
    public struct EntityTypeConstructArgs
    {
        public string EntityType { get; set; }
    }

    public const string MinTypicalMovementSpeed = nameof(MinTypicalMovementSpeed);
    public const string MaxTypicalMovementSpeed = nameof(MaxTypicalMovementSpeed);
    public const string MinTypicalSuspicion = nameof(MinTypicalSuspicion);
    public const string MaxTypicalSuspicion = nameof(MaxTypicalSuspicion);

    private static readonly ConstructPerceptionConfig aggregateConfig;

    static EntityTypeConstructPerceptionManager()
    {
        var builder = new ConstructPerceptionConfigBuilder();

        builder
            .AggregateMax<float>(
                MaxTypicalMovementSpeed,
                EntityConstructPerceptionManager.MaxMovementSpeed
            );
        // .AggregateMax<float>(MaxTypicalSuspicion, "SomeSuspicionKey")
        // .AggregateMin<float>(MinTypicalSuspicion, "SomeSuspicionKey");

        aggregateConfig = builder.Build();
    }

    private void Start()
    {
        // TODO: for debug now
        TouchEntityTypeConstruct("Avrian");
    }

    public PerceptionEntry TouchEntityTypeConstruct(string entityType)
    {
        return EnsureConstructPushed(new() { EntityType = entityType }).construct;
    }

    protected override string GetConstructKey(EntityTypeConstructArgs constructArgs)
    {
        return $"entity-type-construct-{constructArgs.EntityType}";
    }

    protected override bool ShouldBeTrackedByConstruct(
        EntityTypeConstructArgs constructArgs,
        PerceptionEntry construct,
        PerceptionEntry newPerception)
        // We are interested in entity construct perceptions of our type
        => newPerception.Markers.Contains(
                EntityConstructPerceptionManager.EntityConstruct)
            && newPerception.TryGetEntityType(out var newPerceptionEntityType)
            && newPerceptionEntityType == constructArgs.EntityType;

    protected override void HandleConstruct(
        PerceptionEntry hyperConstruct,
        PerceptionEntry relatedConstruct)
    {
        aggregateConfig.ApplyToConstruct(hyperConstruct, relatedConstruct);
    }

    protected override IEnumerable<ConstructPerceptionConfig> GetConfiguration()
    {
        yield return aggregateConfig;
    }

    // protected override void HandleConstructBeforePush(
    //     EntityTypeConstructArgs constructArgs,
    //     PerceptionEntry constructPerception)
    // {
    //     constructPerception.Set(
    //         PerceptionEntryCoreDataKeys.EntityType,
    //         constructArgs.EntityType);
    // }
}
