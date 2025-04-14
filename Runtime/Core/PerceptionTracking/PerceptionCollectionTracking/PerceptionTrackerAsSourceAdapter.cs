using System.Collections.Generic;
using UnityEngine;

public class PerceptionTrackerAsSourceAdapter<TCollection, TReadOnlyCollection> : IPerceptionSource
    where TReadOnlyCollection : IEnumerable<PerceptionEntry>
{
    private readonly PerceptionCollectionTrackerBase<TCollection, TReadOnlyCollection> tracker;

    public string PerceptionSourceKey { get; private set; }

    public PerceptionTrackerAsSourceAdapter(
        string perceptionSourceKey,
        PerceptionCollectionTrackerBase<TCollection, TReadOnlyCollection> tracker)
    {
        PerceptionSourceKey = perceptionSourceKey;
        this.tracker = tracker;
    }

    public IEnumerable<PerceptionEntry> GetPerceptions(params string[] markers)
    {
        return tracker.Collection;
    }
}
