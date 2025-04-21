using System;
using System.Collections.Generic;
using UnityEngine;

public class PerceptionStorageRegistrar : MonoBehaviour
{
    [SerializeField]
    internal SegregatedMemoryManager segregatedMemoryManager;

    [SerializeField]
    private SensoryMemoryManager sensoryMemoryManager;

    [SerializeField]
    private PerceptionTrackingStorage perceptionTrackingStorage;

    public IPerceptionSource EnsureRegistered<TCollection, TReadOnlyCollection>(
        string key,
        Lazy<PerceptionCollectionTrackerBase<TCollection, TReadOnlyCollection>> tracker)
        where TReadOnlyCollection : IEnumerable<PerceptionEntry>
    {
        if (!TryGetResisteredPerceptionSource(key, out var perceptionSource))
        {
            perceptionSource = RegisterStorage(key, tracker.Value);
        }
        return perceptionSource;
    }

    public bool TryGetResisteredPerceptionSource(
        string key,
        out IPerceptionSource perceptionSource)
    {
        return segregatedMemoryManager.TryGetRegisteredPerceptionSource(
            key,
            out perceptionSource);
    }

    public IPerceptionSource RegisterStorage<TCollection, TReadOnlyCollection>(
        string perceptionSourceKey,
        PerceptionCollectionTrackerBase<TCollection, TReadOnlyCollection> tracker,
        Func<TReadOnlyCollection, IEnumerable<PerceptionEntry>> getTrackerPerceptions = null)
    {
        var perceptionSource = new PerceptionTrackerAsSourceAdapter<TCollection, TReadOnlyCollection>(
            perceptionSourceKey,
            tracker,
            getTrackerPerceptions);
        segregatedMemoryManager.RegisterPerceptionSource(
            perceptionSource
        );
        perceptionTrackingStorage.RegisterHandler(tracker);

        return perceptionSource;
    }

    public void RegisterSensoryMemory(ISensoryMemoryStorageCore sensoryMemory)
    {
        sensoryMemoryManager.RegisterSensoryMemory(sensoryMemory);
        segregatedMemoryManager.RegisterPerceptionSource(sensoryMemory);
    }
}
