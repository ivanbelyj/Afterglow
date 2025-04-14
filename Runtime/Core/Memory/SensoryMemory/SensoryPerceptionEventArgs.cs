using UnityEngine;
using System;

public record SensoryPerceptionEventArgs
{
    public string[] IdentifyingMarkers { get; }
    public PerceptionEntry PerceptionEntry { get; }

    public SensoryPerceptionEventArgs(
        string[] identifyingMarkers,
        PerceptionEntry perceptionEntry)
    {
        if (identifyingMarkers == null)
        {
            throw new ArgumentNullException(nameof(identifyingMarkers));
        }

        if (identifyingMarkers.Length == 0)
        {
            throw new ArgumentException(
                "Identifying markers array cannot be empty", 
                nameof(identifyingMarkers));
        }

        IdentifyingMarkers = identifyingMarkers;
        PerceptionEntry = perceptionEntry ?? throw new ArgumentNullException(nameof(perceptionEntry));
    }
}