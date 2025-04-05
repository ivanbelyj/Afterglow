using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

internal class PerceptionMemoryStorageCore 
{
    private readonly List<PerceptionEntry> memoryEntries = new();

    public int Count => memoryEntries.Count;

    public void AddMemory(PerceptionEntry perceptionEntry)
    {
        memoryEntries.Add(perceptionEntry);
    }

    public void AddMemoryRange(IEnumerable<PerceptionEntry> perceptionEntries)
    {
        memoryEntries.AddRange(perceptionEntries);
    }

    // public void AddNotAdded(PerceptionEntry perceptionEntry)
    // {
    //     // TODO: optimize
    //     var existingEntry = memoryEntries.Find(x => x == perceptionEntry);
    //     if (existingEntry == null)
    //     {
    //         AddMemory(perceptionEntry);
    //     }
    // }

    // public void AddNotAddedRange(IEnumerable<PerceptionEntry> perceptionEntries)
    // {
    //     // TODO: optimize
    //     foreach (var entry in perceptionEntries)
    //     {
    //         AddNotAdded(entry);
    //     }
    // }

    public IEnumerable<PerceptionEntry> GetAllForgettable()
    {
        return memoryEntries.Where(x => x.RetentionIntensity.HasValue);
    }

    // public int RemoveAll(Predicate<PerceptionEntry> predicate)
    // {
    //     return memoryEntries.RemoveAll(predicate);
    // }

    public IEnumerable<PerceptionEntry> GetDeepMemory(MemoryParameters memoryParameters)
    {
        return GetMemory(null, memoryParameters.memoryAccessibilityThreshold)
            .Where(x => x.IsDeepMemory(memoryParameters));
    }

    public void Remove(IEnumerable<PerceptionEntry> entries)
    {
        memoryEntries.RemoveAll(entry => entries.Contains(entry));
    }

    public IEnumerable<PerceptionEntry> GetMemory(
        float? minAccessibility,
        float? maxAccessibility,
        params string[] markers)
    {
        markers ??= Array.Empty<string>();

        return memoryEntries.Where(entry =>
            (minAccessibility == null || entry.Accessibility >= minAccessibility) &&
            (maxAccessibility == null || entry.Accessibility < maxAccessibility) &&
            (!markers.Any() || markers.Any(marker => entry.Markers.Contains(marker))));
    }
}
