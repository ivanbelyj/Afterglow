using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;


public class PerceptionMemoryStorage : IPerceptionSource
{
    private readonly string perceptionSourceKey;

    private readonly MemoryParameters memoryParameters;
    private readonly PerceptionMemoryStorageCore perceptionStorage = new();
    private readonly PerceptionEntryOperatingDecorator operatingDecorator;

    internal event EventHandler<PerceptionAddedEventArgs> PerceptionAdded
    {
        add { operatingDecorator.PerceptionAdded += value; }
        remove { operatingDecorator.PerceptionAdded -= value; }
    }
    
    internal MemoryParameters MemoryParameters => memoryParameters;

    // Recovery can be implemented later
    private List<PerceptionEntry> deepMemory = new();

    public PerceptionMemoryStorage(
        string perceptionSourceKey,
        MemoryParameters memoryParameters)
    {
        this.perceptionSourceKey = perceptionSourceKey;
        this.memoryParameters = memoryParameters;
        operatingDecorator = new(this);
    }

    public int MemoryCount => perceptionStorage.Count;
    public int DeepMemoryCount => deepMemory.Count;

    public string PerceptionSourceKey => perceptionSourceKey;

    public int GetWorkMemoryCount() => GetWorkMemory().Count();
    public int GetLongTermMemoryCount() => GetLongTermMemory().Count();

    public void AddOrReplace(string[] identifyingMarkers, PerceptionEntry entry)
    {
        var memoryToRemove = GetMemory(identifyingMarkers).ToList();
        
        if (memoryToRemove.Count > 1)
        {
            Debug.LogError(
                $"Expected to replace 1 perception, but found " +
                $"{memoryToRemove.Count}. Markers: '{string.Join(", ", identifyingMarkers)}'");
        }

        DestructMemory(memoryToRemove);
        
        AddMemory(entry);
    }

    public void AddMemory(PerceptionEntry perceptionEntry)
    {
        perceptionStorage.AddMemory(perceptionEntry);
        operatingDecorator.HandleAddToMemory(perceptionEntry);
    }

    public void AddMemoryRange(IEnumerable<PerceptionEntry> perceptionEntries)
    {
        perceptionStorage.AddMemoryRange(perceptionEntries);
        operatingDecorator.HandleAddToMemory(perceptionEntries);
    }

    /// <summary>
    /// Work memory is a memory type used in most cases
    /// </summary>
    /// <param name="markers">If empty, entries are not filtered by markers</param>
    public IEnumerable<PerceptionEntry> GetWorkMemory(params string[] markers)
    {
        return GetMemoryByAccessibility(
            memoryParameters.workMemoryAccessibilityThreshold,
            null,
            markers);
    }

    /// <summary>
    /// Long-memory is a memory type used in some cases
    /// </summary>
    /// <param name="markers">If empty, entries are not filtered by markers</param>
    public IEnumerable<PerceptionEntry> GetLongTermMemory(params string[] markers)
    {
        return GetMemoryByAccessibility(
            memoryParameters.memoryAccessibilityThreshold,
            memoryParameters.workMemoryAccessibilityThreshold,
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
    /// Work memory text snapshot
    /// </summary>
    public string GetWorkMemoryVerbalRepresentation(bool isDebug = false)
    {
        return GetMemoryVerbalRepresentation(
            memoryParameters.workMemoryAccessibilityThreshold,
            null,
            isDebug);
    }

    /// <summary>
    /// Long-term memory text snapshot
    /// </summary>
    public string GetLongTermMemoryVerbalVerbalRepresentation(bool isDebug = false)
    {
        return GetMemoryVerbalRepresentation(
            memoryParameters.memoryAccessibilityThreshold,
            memoryParameters.workMemoryAccessibilityThreshold,
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
        foreach (var entry in perceptionStorage.GetAllForgettable())
        {
            entry.Accessibility = Forget(entry, simulationTickSeconds);
        }
        HandleAddDeepMemory();
        HandleClearForgottenMemory();
    }

    public float Forget(PerceptionEntry entry, float simulationTickSeconds)
        => Mathf.Clamp01(entry.Accessibility - simulationTickSeconds / entry.RetentionIntensity.Value);

    public string GetMemoryVerbalRepresentation(
        float? minAccessibility,
        float? maxAccessibility,
        bool isDebug)
        => string.Join(
            "\n",
            GetMemoryByAccessibility(minAccessibility, maxAccessibility)
                .Select(e => isDebug ? e.ToString() : e.VerbalRepresentation));

    public IEnumerable<PerceptionEntry> GetMemory(params string[] markers)
    {
        return GetMemoryByAccessibility(
            null,
            null,
            markers
        );
    }

    IEnumerable<PerceptionEntry> IPerceptionSource.GetPerceptions(
        params string[] markers)
    {
        return GetMemory(markers);
    }

    public IEnumerable<PerceptionEntry> GetMemoryByAccessibility(
        float? minAccessibility,
        float? maxAccessibility,
        params string[] markers)
    {
        return perceptionStorage.GetMemory(minAccessibility, maxAccessibility, markers);
    }

    private void HandleAddDeepMemory()
    {
        var deepMemory = perceptionStorage
            .GetDeepMemory(memoryParameters)
            .ToList();
        deepMemory.AddRange(deepMemory);
    }

    private void HandleClearForgottenMemory()
    {
        var memoryToClear = GetMemoryByAccessibility(null, memoryParameters.memoryAccessibilityThreshold)
            .ToList();
        DestructMemory(memoryToClear);
    }

    private void DestructMemory(List<PerceptionEntry> entries)
    {
        perceptionStorage.Remove(entries);
        operatingDecorator.HandleDestruct(entries);
    }
}