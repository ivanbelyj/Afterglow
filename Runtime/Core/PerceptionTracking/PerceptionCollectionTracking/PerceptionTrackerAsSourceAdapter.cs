using System;
using System.Collections.Generic;
using UnityEngine;

public class PerceptionTrackerAsSourceAdapter<TCollection, TReadOnlyCollection> : IPerceptionSource
{
    private readonly PerceptionCollectionTrackerBase<TCollection, TReadOnlyCollection> tracker;
    private readonly Func<TReadOnlyCollection, IEnumerable<PerceptionEntry>> getTrackerPerceptions;

    public string PerceptionSourceKey { get; private set; }

    public PerceptionTrackerAsSourceAdapter(
        string perceptionSourceKey,
        PerceptionCollectionTrackerBase<TCollection, TReadOnlyCollection> tracker,
        Func<TReadOnlyCollection, IEnumerable<PerceptionEntry>> getTrackerPerceptions = null)
    {
        PerceptionSourceKey = perceptionSourceKey;
        this.tracker = tracker;
        this.getTrackerPerceptions = getTrackerPerceptions;
    }

    public IEnumerable<PerceptionEntry> GetPerceptions(params string[] markers)
    {
        if (getTrackerPerceptions == null)
        {
            if (tracker.Collection is IEnumerable<PerceptionEntry> enumerable)
            {
                return enumerable;
            }
            Debug.LogError(
                $"tracker's collection doesn't implement {nameof(IEnumerable<PerceptionEntry>)}, " +
                $"but {nameof(getTrackerPerceptions)} was not provided");
        }
        
        return getTrackerPerceptions(tracker.Collection);
    }
}
