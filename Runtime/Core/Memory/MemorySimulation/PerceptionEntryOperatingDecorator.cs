using System;
using System.Collections.Generic;
using UnityEngine;

internal class PerceptionEntryOperatingDecorator
{
    public event EventHandler<PerceptionAddedEventArgs> PerceptionAdded;

    private readonly PerceptionMemoryStorage perceptionStorage;

    public PerceptionEntryOperatingDecorator(
        PerceptionMemoryStorage perceptionStorage)
    {
        this.perceptionStorage = perceptionStorage;
    }

    public void HandleAddToMemory(IEnumerable<PerceptionEntry> entries)
    {
        foreach (var entry in entries)
        {
            HandleAddToMemory(entry);
        }
    }

    public void HandleAddToMemory(PerceptionEntry entry)
    {
        entry.PerceptionStateChanges?.Invoke(
            this,
            new (entry, null, entry.ClassifyStateInMemory(perceptionStorage.MemoryParameters)));
        PerceptionAdded?.Invoke(this, new() {
            PerceptionEntry = entry
        });
    }

    public void HandleDestruct(IEnumerable<PerceptionEntry> entries)
    {
        foreach (var entry in entries)
        {
            HandleDestruct(entry);
        }
    }

    public void HandleDestruct(PerceptionEntry entry)
    {
        entry.PerceptionStateChanges?.Invoke(
            this,
            new(
                entry,
                entry.ClassifyStateInMemory(perceptionStorage.MemoryParameters),
                PerceptionState.Destructed));
        entry.Destruct();
    }
}
