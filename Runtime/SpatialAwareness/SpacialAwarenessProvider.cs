using UnityEngine;

using static PerceptionEntryMarkers;

/// <summary>
/// Perception storage decorator
/// </summary>
public class SpacialAwarenessStorage
{
    public const string PositionPerceptionDataKey = "Position";
    public const string TransitionStartPerceptionDataKey = "TransitionFrom";
    public const string TransitionEndPerceptionDataKey = "TransitionTo";

    private readonly PerceptionStorage perceptionStorage;

    public SpacialAwarenessStorage(PerceptionStorage perceptionStorage)
    {
        this.perceptionStorage = perceptionStorage;
    }

    public void MemorizePosition(
        PerceptionEntry perceptionEntry,
        Vector3 position,
        params string[] markers)
    {
        SetPositionData(perceptionEntry, position, markers);
        perceptionStorage.AddMemory(perceptionEntry);
    }

    public void MemorizeTransition(
        PerceptionEntry perceptionEntry,
        Vector3 from,
        Vector3 to,
        params string[] markers)
    {
        SetTransitionData(perceptionEntry, from, to, markers);
        perceptionStorage.AddMemory(perceptionEntry);
    }

    private void SetPositionData(
        PerceptionEntry perceptionEntry,
        Vector3 position,
        params string[] markers)
    {
        perceptionEntry.PerceptionData[PositionPerceptionDataKey] = position;
        perceptionEntry.Markers.Add(Position);
        foreach (var marker in markers) 
        {
            perceptionEntry.Markers.Add(marker);
        }
    }

    private void SetTransitionData(
        PerceptionEntry perceptionEntry,
        Vector3 from,
        Vector3 to,
        params string[] markers)
    {
        perceptionEntry.PerceptionData[TransitionStartPerceptionDataKey] = from;
        perceptionEntry.PerceptionData[TransitionEndPerceptionDataKey] = to;
        
        perceptionEntry.Markers.Add(Transition);
        foreach (var marker in markers) 
        {
            perceptionEntry.Markers.Add(marker);
        }
    }
}

public interface ISpacialAwarenessProvider
{

}

/// <summary>
/// Provides data to perception-based navigation
/// </summary>
[RequireComponent(typeof(MemorySimulator))]
public class SpacialAwarenessProvider : MonoBehaviour, ISpacialAwarenessProvider
{
    private MemorySimulator memorySimulator;
    private SpacialAwarenessStorage spacialAwarenessStorage;

    private void Awake()
    {
        memorySimulator = GetComponent<MemorySimulator>();
        spacialAwarenessStorage = new SpacialAwarenessStorage(memorySimulator.PerceptionStorage);
    }

    public void MemorizeNewPosition(
        string perceptionVerbalRepresentation,
        float retentionIntensity,
        Vector3 gridPosition,
        double? timestamp,
        params string[] markers)
    {
        MemorizePosition(
            new PerceptionEntry(perceptionVerbalRepresentation, timestamp)
            {
                Accessibility = 1f,
                RetentionIntensity = retentionIntensity
            },
            gridPosition,
            markers);
    }

    public void MemorizePosition(
        PerceptionEntry perceptionEntry,
        Vector3 position,
        params string[] markers)
    {
        spacialAwarenessStorage.MemorizePosition(perceptionEntry, position, markers);
    }

    public void MemorizeTransition(
        PerceptionEntry perceptionEntry,
        Vector3 from,
        Vector3 to,
        params string[] markers)
    {
        spacialAwarenessStorage.MemorizeTransition(perceptionEntry, from, to, markers);
    }
}
