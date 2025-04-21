using System;
using System.Collections.Generic;
using UnityEngine;

using static PerceptionEntryCoreDataKeys;

public delegate void PerceptionConstructHandler(
    PerceptionEntry construct,
    PerceptionEntry newPerception);

public delegate bool PerceptionConstructPredicate(
    PerceptionEntry construct,
    PerceptionEntry newPerception);

public class PerceptionConstructGate : MonoBehaviour, IConstructPerceptionProvider
{
    public const string HyperConstructMarker = "HyperConstruct";

    [SerializeField, Required]
    private MemorySimulator memorySimulator;

    [SerializeField, Required]
    private InterfaceField<IPerceptionTrackingStorage> perceptionTrackingStorage;

    [SerializeField, Required]
    private PerceptionStorageRegistrar perceptionStorageRegistrar;

    [SerializeField, Required]
    private InterfaceField<ISegregatedMemoryProvider> segregatedMemoryProvider;

    private PerceptionDictionaryTracker<string> constructPerceptionsByKey;
    private ConstructPerceptionCompoundTracker constructCompoundTracker;

    private void Awake()
    {
        constructPerceptionsByKey = new PerceptionDictionaryTracker<string>(
            x => x.PerceptionData.ContainsKey(Construct),
            x => x.Get<string>(Construct));

        perceptionStorageRegistrar.RegisterStorage(
            CoreSegregatedPerceptionSources.ConstructPerceptions,
            constructPerceptionsByKey,
            trackerCollection => trackerCollection.Values);

        constructCompoundTracker = new ConstructPerceptionCompoundTracker();
        perceptionTrackingStorage.Value.RegisterHandler(constructCompoundTracker);
    }

    public PerceptionEntry EnsureConstructPushed(
        string constructKey,
        PerceptionConstructPredicate shouldBeTrackedByConstruct,
        PerceptionConstructHandler handleConstruct,
        Action<PerceptionEntry> handleConstructBeforePush)
    {
        if (!TryGetConstruct(constructKey, out var constructPerception))
        {
            constructPerception = PushConstruct(
                constructKey,
                shouldBeTrackedByConstruct,
                handleConstruct,
                handleConstructBeforePush);
        }
        return constructPerception;
    }

    public PerceptionEntry GetConstruct(string constructKey)
    {
        return constructPerceptionsByKey.Collection[constructKey];
    }

    public bool TryGetConstruct(string constructKey, out PerceptionEntry perceptionEntry)
    {
        return constructPerceptionsByKey.Collection.TryGetValue(
            constructKey,
            out perceptionEntry);
    }

    public void UpdateConstruct(
        string constructKey,
        IEnumerable<PerceptionEntry> newPerceptions)
    {
        if (!constructCompoundTracker.TryGetTracker(constructKey, out var tracker))
        {
            Debug.LogError($"Cannot force update construct: tracker not found");
        }

        foreach (var perception in newPerceptions)
        {
            tracker.HandleRelatedPerception(perception);
        }
    }

    public PerceptionEntry PushConstruct(
        string constructKey,
        PerceptionConstructPredicate shouldBeTrackedByConstruct,
        PerceptionConstructHandler handleConstruct,
        Action<PerceptionEntry> handleConstructBeforePush)
    {
        return PushConstructCore(
            constructKey,
            shouldBeTrackedByConstruct,

            // Make our own callback to support hyper-constructs
            (construct, newPerception) =>
            {
                // We received a perceptions, that is a construct
                if (newPerception.TryGetConstructKey(out var relatedConstructKey))
                {
                    if (!constructCompoundTracker.TryGetTracker(
                        relatedConstructKey,
                        out var relatedTracker))
                    {
                        Debug.LogError(
                            $"Found construct key in a construct perception " +
                            $"related to the hyper-construct, but tracker " +
                            $"was not found by the given key");
                    }

                    // Typically for debug
                    construct.Markers.Add(HyperConstructMarker);

                    // Subscribe our construct (hyper-construct) to the related
                    // construct updates

                    // If related construct was updated (via tracking
                    // or explicit update call), we should update hyber-construct
                    relatedTracker.HandleHyperConstructSubscription(
                        constructKey,
                        construct,
                        (relatedConstruct) => handleConstruct(construct, relatedConstruct));
                }
                handleConstruct(construct, newPerception);
            },
            handleConstructBeforePush
        );
    }

    private PerceptionEntry PushConstructCore(
        string constructKey,
        PerceptionConstructPredicate shouldBeTrackedByConstruct,
        PerceptionConstructHandler handleConstruct,
        Action<PerceptionEntry> handleConstructBeforePush)
    {
        var perception = CreateConstructPerception(constructKey);

        constructCompoundTracker.AddTracker(
            new ConstructPerceptionTracker(
                perception,
                shouldBeTrackedByConstruct,
                handleConstruct));

        handleConstructBeforePush?.Invoke(perception);

        PushPerception(perception);

        return perception;
    }

    private void PushPerception(PerceptionEntry perception)
    {
        perceptionTrackingStorage.Value.Track(perception);
        memorySimulator.AddMemory(perception);
    }

    private PerceptionEntry CreateConstructPerception(string constructKey)
    {
        var gameTime = GameTimeProvider.Instance.GetGameTime();

        // Construct typically has creation time only
        return new(gameTime, null)
        {
            // Constructs are skipped by memory by default
            // (typically construct accumulates many perceptions
            // so it is quite long-living thing)
            RetentionIntensity = null,
            PerceptionData = {
                [Construct] = constructKey
            }
        };
    }
}
