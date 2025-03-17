using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public record PerceptedSightData
{
    public float distance;
    public double timestamp;
}

/// <summary>
/// Stores currently percepted vision data
/// </summary>
public class VisionSensoryMemoryStorage : ISensoryMemoryStorage
{
    public const string VisionPerceptionDataKey = "Vision";

    private readonly Dictionary<Sight, PerceptedSightData> dataBySight = new();
    private readonly IGameTimeProvider gameTimeProvider;

    public event EventHandler<SensoryPerceptionReleasedEventArgs> SensoryPerceptionReleased;

    public VisionSensoryMemoryStorage(IGameTimeProvider gameTimeProvider)
    {
        this.gameTimeProvider = gameTimeProvider;
    }

    public void CaptureSight(Sight sight, float distance)
    {
        var perceptedSightData = new PerceptedSightData()
        {
            distance = distance,
            timestamp = NetworkTime.time
        };

        Debug.Log($"Captured sight: " + sight.VerbalRepresentation + $" [{sight.transform.parent.gameObject.name}]");

        if (dataBySight.ContainsKey(sight))
        {
            dataBySight[sight] = perceptedSightData;
        }
        else 
        {
            dataBySight.Add(sight, perceptedSightData);
        }
    }
    
    public void ReleaseSight(Sight sight)
    {
        Debug.Log($"Released sight: " + sight.VerbalRepresentation + $" [{sight.transform.parent.gameObject.name}]");

        dataBySight.Remove(sight);
        SensoryPerceptionReleased?.Invoke(this, new() {
            perceptionEntry = ToMemory(sight)
        });
    }

    private PerceptionEntry ToMemory(Sight sight)
    {
        // Todo:
        return new(GetVisionMemoryVerbalRepresentation(sight), gameTimeProvider.GetGameTime()) {
            Accessibility = 1f,
            Markers = new HashSet<string>() { VisionPerceptionDataKey },
            PerceptionData = new() {
                
            },
            RetentionIntensity = 30f,
        };
    }

    private string GetVisionMemoryVerbalRepresentation(Sight sight)
    {
        return $"I saw the following sight: {sight.VerbalRepresentation}";
    }
}
