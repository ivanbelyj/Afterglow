using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public sealed class PerceptionListTracker : PerceptionCollectionTrackerBase<List<PerceptionEntry>, ReadOnlyCollection<PerceptionEntry>>
{
    private readonly List<PerceptionEntry> list = new();

    public PerceptionListTracker(Func<PerceptionEntry, bool> shouldTrack) : base(shouldTrack)
    {
    }

    protected override ReadOnlyCollection<PerceptionEntry> AsReadOnly() => list.AsReadOnly();
    protected override void AddToCollection(PerceptionEntry entry) => list.Add(entry);
    protected override void RemoveFromCollection(PerceptionEntry entry) => list.Remove(entry);
}