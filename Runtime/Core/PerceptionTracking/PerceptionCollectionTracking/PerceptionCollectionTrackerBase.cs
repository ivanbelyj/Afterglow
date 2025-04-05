using System;
using System.Collections.Generic;

public abstract class PerceptionCollectionTrackerBase<TCollection, TReadOnlyCollection>
    : IPerceptionTrackingHandler
{
    public event EventHandler<PerceptionEntry> PerceptionAdded;
    public event EventHandler<PerceptionEntry> PerceptionRemoved;

    private readonly Func<PerceptionEntry, bool> shouldTrack;

    protected abstract void AddToCollection(PerceptionEntry entry);
    protected abstract void RemoveFromCollection(PerceptionEntry entry);
    protected abstract TReadOnlyCollection AsReadOnly();

    public TReadOnlyCollection Collection => AsReadOnly();

    public PerceptionCollectionTrackerBase(
        Func<PerceptionEntry, bool> shouldTrack)
    {
        this.shouldTrack = shouldTrack;
    }

    /// <summary>
    /// Adds perception if it requires tracker's conditions
    /// </summary>
    public void Track(PerceptionEntry perception)
    {
        if (shouldTrack(perception))
        {
            Add(perception);
        }
    }

    protected void Add(PerceptionEntry entry)
    {
        AddToCollection(entry);
        entry.PerceptionStateChanges += OnPerceptionStateChanged;
        PerceptionAdded?.Invoke(this, entry);
    }

    protected void Remove(PerceptionEntry entry)
    {
        RemoveFromCollection(entry);
        entry.PerceptionStateChanges -= OnPerceptionStateChanged;
        PerceptionRemoved?.Invoke(this, entry);
    }

    protected void OnPerceptionStateChanged(object sender, PerceptionStateChangedEventArgs e)
    {
        if (e.NewState == PerceptionState.Destructed)
        {
            Remove(e.PerceptionEntry);
        }
    }
}