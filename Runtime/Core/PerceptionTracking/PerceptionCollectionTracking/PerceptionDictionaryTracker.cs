using System;
using System.Collections.Generic;

public sealed class PerceptionDictionaryTracker<TKey> :
    PerceptionCollectionTrackerBase<Dictionary<PerceptionEntry, TKey>, IReadOnlyDictionary<TKey, PerceptionEntry>>
{
    private readonly Dictionary<TKey, PerceptionEntry> dictionary;
    private readonly Func<PerceptionEntry, TKey> getKey;

    public PerceptionDictionaryTracker(
        Func<PerceptionEntry, bool> shouldTrack,
        Func<PerceptionEntry, TKey> getKey) : base(shouldTrack)
    {
        this.getKey = getKey;
    }

    protected override void AddToCollection(PerceptionEntry entry)
    {
        var key = getKey(entry);
        dictionary.Add(key, entry);
    }

    protected override IReadOnlyDictionary<TKey, PerceptionEntry> AsReadOnly()
    {
        return dictionary;
    }

    protected override void RemoveFromCollection(PerceptionEntry entry)
    {
        dictionary.Remove(getKey(entry));
    }
}