using System;
using System.Collections.Generic;
using UnityEngine;

public class PerceptionEntry : IUntypedStorage
{
    /// <summary>
    /// Typically called by memory simulation system
    /// </summary>
    public EventHandler<PerceptionStateChangedEventArgs> PerceptionStateChanges { get; set; }

    #region Fields
    // TODO: Make it not readonly and maybe build it dynamically
    private readonly string verbalRepresentationCore;
    private readonly PerceptionTimeDistance? timeDistance;

    private Dictionary<string, object> perceptionData;
    private HashSet<string> markers;
    private float accessibility = 1f;
    private float? retentionIntensity = 0;

    private double? timestampFrom;
    private double? timestampTo;

    private bool isDestructed = false;
    #endregion

    #region Properties
    public double? TimestampFrom
    {
        get
        {
            EnsureNotDestructed();
            return timestampFrom;
        }
        set
        {
            EnsureNotDestructed();
            timestampFrom = value;
        }
    }

    public double? TimestampTo
    {
        get
        {
            EnsureNotDestructed();
            return timestampTo;
        }
        set
        {
            EnsureNotDestructed();
            timestampTo = value;
        }
    }

    public double? Timestamp => TimestampTo ?? TimestampFrom;

    public PerceptionTimeDistance? TimeDistance
    {
        get
        {
            EnsureNotDestructed();
            if (timeDistance == null && TimestampTo != null)
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
            EnsureNotDestructed();
            return verbalRepresentationCore;
        }
    }

    public HashSet<string> Markers
    {
        get
        {
            EnsureNotDestructed();
            markers ??= new();
            return markers;
        }
        set
        {
            EnsureNotDestructed();
            markers = value ?? throw new ArgumentNullException(
                $"{nameof(Markers)} cannot be set to null");
        }
    }

    /// <summary>
    /// Controls the speed of the transition of perception between memory states.
    /// Set for temporary perceptions only. Null for memories that are considered
    /// permanent on the scale of the simulation.
    /// </summary>
    public float? RetentionIntensity
    {
        get
        {
            EnsureNotDestructed();
            return retentionIntensity;
        }
        set
        {
            EnsureNotDestructed();
            retentionIntensity = value;
        }
    }

    /// <summary>
    /// <para>
    /// A numerical indicator of the perception accessibility.
    /// Its interpretation into a <see cref="PerceptionState"/>
    /// depends on the parameters of a particular memory system.
    /// </para>
    /// 
    /// <para>
    /// Typically it's decreased constantly by the memory simulation,
    /// if <see cref="RetentionIntensity"/> is not null
    /// (or can be increased in certain cases).
    /// </para>
    /// 
    /// <para>
    /// Values from 0 (inclusive) to 1 (inclusive).
    /// Typically, '0' is indicating destructed memory,
    /// and '1' - sensory memory, percepted directly in real-time
    /// </para>
    /// </summary>
    public float Accessibility
    {
        get
        {
            EnsureNotDestructed();
            return accessibility;
        }
        set
        {
            EnsureNotDestructed();

            if (value < 0f || value > 1f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    $"{nameof(Accessibility)} must be between 0 and 1.");
            }
            accessibility = value;
        }
    }

    public bool IsDestructed => isDestructed;

    public Dictionary<string, object> PerceptionData
    {
        get
        {
            EnsureNotDestructed();
            perceptionData ??= new();
            return perceptionData;
        }
        set
        {
            EnsureNotDestructed();
            perceptionData = value ?? throw new ArgumentNullException(
                $"{nameof(PerceptionEntry)} cannot be set to null");
        }
    }

    Dictionary<string, object> IUntypedStorage.Data => PerceptionData;

    #endregion

    #region Constructors
    internal PerceptionEntry(double? timestampFrom, double? timestampTo)
    {
        this.timestampFrom = timestampFrom;
        this.timestampTo = timestampTo;
    }

    internal PerceptionEntry(PerceptionTimeDistance timeDistance)
    {
        this.timeDistance = timeDistance;
    }
    #endregion

    public void Destruct()
    {
        EnsureNotDestructed();
        PerceptionStateChanges = null;
        isDestructed = true;
    }

    public override string ToString()
    {
        EnsureNotDestructed();
        return $"{VerbalRepresentation} (a: {Accessibility:F3}, r: {RetentionIntensity?.ToString() ?? "null"})";
    }

    private void EnsureNotDestructed()
    {
        if (isDestructed)
        {
            Debug.LogError(
                $"Trying to access perception that has '{PerceptionState.Destructed}' state " +
                "set during the memory simulation. Please, don't use destructed memories " +
                "and avoid storing any references to allow GC collect them " +
                "and prevent memory waste");
        }
    }
}
