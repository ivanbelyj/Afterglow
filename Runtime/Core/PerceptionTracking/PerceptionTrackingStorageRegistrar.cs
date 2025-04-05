using System.Collections.Generic;
using UnityEngine;

public class PerceptionTrackingStorageRegistrar : MonoBehaviour
{
    [SerializeField]
    private SegregatedMemoryManager segregatedMemoryManager;

    [SerializeField]
    private PerceptionTrackingStorage perceptionTrackingStorage;

    public void RegisterStorage<TCollection, TReadOnlyCollection>(
        uint perceptionSourceLayerMask,
        PerceptionCollectionTrackerBase<TCollection, TReadOnlyCollection> tracker)
        where TReadOnlyCollection : IEnumerable<PerceptionEntry>
    {
        segregatedMemoryManager.RegisterPerceptionSource(
            new PerceptionTrackerAsSourceAdapter<TCollection, TReadOnlyCollection>(
                perceptionSourceLayerMask,
                tracker)
        );
        perceptionTrackingStorage.RegisterHandler(tracker);
    }
}
