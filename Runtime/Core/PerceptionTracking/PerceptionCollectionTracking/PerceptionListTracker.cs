using System;
using System.Collections.Generic;

public sealed class PerceptionListTracker : PerceptionCollectionTrackerBase<List<PerceptionEntry>, IReadOnlyList<PerceptionEntry>>
{
    private readonly List<PerceptionEntry> list = new();

    public PerceptionListTracker(Func<PerceptionEntry, bool> shouldTrack) : base(shouldTrack)
    {
    }

    protected override IReadOnlyList<PerceptionEntry> AsReadOnly() => list.AsReadOnly();
    protected override void AddToCollection(PerceptionEntry entry) => list.Add(entry);
    protected override void RemoveFromCollection(PerceptionEntry entry) => list.Remove(entry);
}