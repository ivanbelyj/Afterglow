using System.Collections.Generic;
using UnityEngine;

public class PerceptionTrackerAsSourceAdapter<TCollection, TReadOnlyCollection> : IPerceptionSource
    where TReadOnlyCollection : IEnumerable<PerceptionEntry>
{
    private readonly PerceptionCollectionTrackerBase<TCollection, TReadOnlyCollection> tracker;

    public uint PerceptionSourceLayerMask { get; private set; }

    public PerceptionTrackerAsSourceAdapter(
        uint perceptionSourceLayerMask,
        PerceptionCollectionTrackerBase<TCollection, TReadOnlyCollection> tracker)
    {
        PerceptionSourceLayerMask = perceptionSourceLayerMask;
        this.tracker = tracker;
    }

    public IEnumerable<PerceptionEntry> GetPerceptions(params string[] markers)
    {
        return tracker.Collection;
    }
}
