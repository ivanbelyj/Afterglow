using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PerceptionStorageCore 
{
    private readonly List<PerceptionEntry> memoryEntries = new();

    public int Count => memoryEntries.Count;

    public void AddMemory(PerceptionEntry[] perceptionEntries)
    {
        memoryEntries.AddRange(perceptionEntries);
    }

    public IEnumerable<PerceptionEntry> GetAllForgettable()
    {
        return memoryEntries.Where(x => x.RetentionIntensity.HasValue);
    }

    public IEnumerable<PerceptionEntry> GetDeepMemory(MemoryParameters memoryParameters) {
        return GetMemory(null, memoryParameters.memoryAccessibilityThreshold)
            .Where(x => x.IsDeepMemory(memoryParameters));
    }

    public void ClearLessAccessibleThan(float threshold)
    {
        memoryEntries.RemoveAll(entry => entry.Accessibility < threshold);
    }

    public IEnumerable<PerceptionEntry> GetMemory(
        float? minAccessibility,
        float? maxAccessibility,
        params string[] markers)
    {
        return memoryEntries.Where(entry =>
            (minAccessibility == null || entry.Accessibility >= minAccessibility) &&
            (maxAccessibility == null || entry.Accessibility < maxAccessibility) &&
            (markers.Length == 0 || markers.Any(marker => entry.Markers.Contains(marker))));
    }
}

public class PerceptionStorage
{
    private readonly MemoryParameters memoryParameters;
    private readonly PerceptionStorageCore perceptionStorageCore = new();

    // Recovery can be implemented later
    private List<PerceptionEntry> deepMemory = new();

    public PerceptionStorage(MemoryParameters memoryParameters)
    {
        this.memoryParameters = memoryParameters;
    }

    public int MemoryCount => perceptionStorageCore.Count;
    public int DeepMemoryCount => deepMemory.Count;

    public int GetActiveMemoryCount() => GetActiveMemory().Count();
    public int GetPassiveMemoryCount() => GetPassiveMemory().Count();

    public void AddMemory(params PerceptionEntry[] perceptionEntries)
    {
        perceptionStorageCore.AddMemory(perceptionEntries);
    }

    /// <summary>
    /// Active memory is a memory type used in most cases
    /// </summary>
    /// <param name="markers">If empty, entries are not filtered by markers</param>
    public IEnumerable<PerceptionEntry> GetActiveMemory(params string[] markers)
    {
        return GetMemory(
            memoryParameters.activeMemoryAccessibilityThreshold,
            null,
            markers);
    }

    /// <summary>
    /// Passive memory is a memory type used in some cases
    /// </summary>
    /// <param name="markers">If empty, entries are not filtered by markers</param>
    public IEnumerable<PerceptionEntry> GetPassiveMemory(params string[] markers)
    {
        return GetMemory(
            memoryParameters.memoryAccessibilityThreshold,
            memoryParameters.activeMemoryAccessibilityThreshold,
            markers);
    }

    /// <summary>
    /// Deep memory is a type of memory very rarely used in the game,
    /// but it is an opportunity to recover memories.
    /// Typically only the most important memories can persist
    /// as deep memory, and it is not involved in simulation
    /// (so it cannot be removed)
    /// </summary>
    public IEnumerable<PerceptionEntry> GetDeepMemory() {
        return deepMemory;
    }

    /// <summary>
    /// Active memory text snapshot
    /// </summary>
    public string GetActiveMemoryVerbalRepresentation(bool isDebug = false)
    {
        return GetMemoryVerbalRepresentation(
            memoryParameters.activeMemoryAccessibilityThreshold,
            null,
            isDebug);
    }

    /// <summary>
    /// Passive memory text snapshot
    /// </summary>
    public string GetPassiveMemoryVerbalVerbalRepresentation(bool isDebug = false)
    {
        return GetMemoryVerbalRepresentation(
            memoryParameters.memoryAccessibilityThreshold,
            memoryParameters.activeMemoryAccessibilityThreshold,
            isDebug);
    }

    /// <summary>
    /// Performs memory simulation, leading to memory decay (forgetting)
    /// </summary>
    /// <param name="simulationTickSeconds">
    /// Simulation tick call frequency in seconds
    /// </param>
    public void TickMemoryDecay(float simulationTickSeconds)
    {
        foreach (var entry in perceptionStorageCore.GetAllForgettable())
        {
            entry.Accessibility = Forget(entry, simulationTickSeconds);
        }
        var deepMemory = perceptionStorageCore
            .GetDeepMemory(memoryParameters)
            .ToList();
        deepMemory.AddRange(deepMemory);
        perceptionStorageCore.ClearLessAccessibleThan(memoryParameters.memoryAccessibilityThreshold);
    }

    public float Forget(PerceptionEntry entry, float simulationTickSeconds)
        => Mathf.Clamp01(entry.Accessibility - simulationTickSeconds / entry.RetentionIntensity.Value);

    public string GetMemoryVerbalRepresentation(
        float? minAccessibility,
        float? maxAccessibility,
        bool isDebug)
        => string.Join(
            "\n",
            GetMemory(minAccessibility, maxAccessibility)
                .Select(e => isDebug ? e.ToString() : e.VerbalRepresentation));

    public IEnumerable<PerceptionEntry> GetMemory(
        float? minAccessibility,
        float? maxAccessibility,
        params string[] markers)
    {
        return perceptionStorageCore.GetMemory(minAccessibility, maxAccessibility, markers);
    }
}