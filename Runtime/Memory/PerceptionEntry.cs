using System;
using System.Collections.Generic;
using UnityEngine;

public enum PerceptionTimeDistance
{
    RightNow,
    Recently,
    Earlier,
    QuiteAWhileAgo,
    LongAgo,
    VeryLongAgo,
    AtAnUnconsciousAge
}

public class PerceptionEntry
{
    private readonly string verbalRepresentationCore;
    private readonly PerceptionTimeDistance? timeDistance;

    private Dictionary<string, object> perceptionData;
    private HashSet<string> markers;
    private float accessibility;

    public double? Timestamp { get; }

    public PerceptionTimeDistance? TimeDistance {
        get
        {
            if (timeDistance == null && Timestamp != null)
            {
                // Todo: convert it
                return PerceptionTimeDistance.RightNow;
            }
            return timeDistance;
        }
    }

    /// <summary>
    /// It is used primarily for LLM and human-readable debugging messages
    /// </summary>
    public string VerbalRepresentation
    {
        get
        {
            return verbalRepresentationCore;
        }
    }

    public HashSet<string> Markers {
        get
        {
            markers ??= new();
            return markers;
        }
        set
        {
            markers = value ?? throw new ArgumentNullException(
                $"{nameof(Markers)} cannot be set to null");
        }
    }

    /// <summary>
    /// Controls the speed of transition from work memory to long-term memory.
    /// Set for temporary perceptions only, null for memories that are considered
    /// permanent on the scale of the game.
    /// </summary>
    public float? RetentionIntensity { get; set; }

    /// <summary>
    /// Higher values for more important or obsessive memories.
    /// Decreasing constantly if <see cref="RetentionIntensity"/> is not null
    /// (or can be increased in certain cases).
    /// From 0 (inclusive) to 1 (inclusive)
    /// </summary>
    public float Accessibility
    {
        get => accessibility;
        set
        {
            if (value < 0f || value > 1f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    $"{nameof(Accessibility)} must be between 0 and 1.");
            }
            accessibility = value;
        }
    }

    public bool IsDeepMemory(MemoryParameters memoryParameters)
        => Accessibility < memoryParameters.memoryAccessibilityThreshold
        && RetentionIntensity == null;

    public Dictionary<string, object> PerceptionData {
        get
        {
            perceptionData ??= new();
            return perceptionData;
        }
        set
        {
            perceptionData = value ?? throw new ArgumentNullException(
                $"{nameof(PerceptionEntry)} cannot be set to null");
        }
    }

    public PerceptionEntry(string verbalRepresentation, double? timestamp)
    {
        verbalRepresentationCore = verbalRepresentation;
        Timestamp = timestamp;
    }

    public PerceptionEntry(string verbalRepresentation, PerceptionTimeDistance timeDistance)
    {
        verbalRepresentationCore = verbalRepresentation;
        this.timeDistance = timeDistance;
    }

    public override string ToString()
    {
        return $"{VerbalRepresentation} (a: {Accessibility:F3}, r: {RetentionIntensity?.ToString() ?? "null"})";
    }
}
