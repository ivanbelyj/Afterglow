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

    public IPerceptionSource RegisterStorage<TCollection, TReadOnlyCollection>(
        string perceptionSourceKey,
        PerceptionCollectionTrackerBase<TCollection, TReadOnlyCollection> tracker)
        where TReadOnlyCollection : IEnumerable<PerceptionEntry>
    {
        var perceptionSource = new PerceptionTrackerAsSourceAdapter<TCollection, TReadOnlyCollection>(
            perceptionSourceKey,
            tracker);
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
