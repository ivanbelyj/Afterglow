using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

using static PerceptionMarkerUtilsCore;

/// <summary>
/// Stores currently percepted vision data
/// </summary>
public class VisionSensoryMemoryStorage : ISensoryMemoryStorage
{
    public event EventHandler<SensoryPerceptionEventArgs> SensoryPerceptionReleased;

    public event EventHandler<VisionSensoryMemoryEventArgs> SightCaptured;
    public event EventHandler<VisionSensoryMemoryEventArgs> SightReleased;

    private readonly Dictionary<Guid, PerceptedSightData> perceptionSightDataByEntityId = new();
    private readonly IRecognizer<Sight> perceptionRecognizer;

    public uint PerceptionSourceLayerMask => (uint)SegregatedPerceptionLayerCore.VisionSensoryMemory;

    public VisionSensoryMemoryStorage(IRecognizer<Sight> perceptionRecognizer)
    {
        this.perceptionRecognizer = perceptionRecognizer;
    }

    public void CaptureSight(Sight sight, float distance)
    {
        var perceptedSightData = new PerceptedSightData()
        {
            Distance = distance,
            PerceptionEntry = ToPerception(sight)
        };

        // Debug.Log($"Captured sight: " + sight.VerbalRepresentation + $" [{sight.transform.parent.gameObject.name}]");

        if (perceptionSightDataByEntityId.ContainsKey(sight.EntityId))
        {
            perceptionSightDataByEntityId[sight.EntityId] = perceptedSightData;
        }
        else 
        {
            perceptionSightDataByEntityId.Add(sight.EntityId, perceptedSightData);
        }

        SightCaptured?.Invoke(this, new() {
            PerceptedSightData = perceptedSightData,
            Sight = sight
        });
    }
    
    public void ReleaseSight(Sight sight)
    {
        // Debug.Log($"Released sight: " + sight.VerbalRepresentation + $" [{sight.transform.parent.gameObject.name}]");
        var data = perceptionSightDataByEntityId[sight.EntityId];
        perceptionSightDataByEntityId.Remove(sight.EntityId);
        SensoryPerceptionReleased?.Invoke(
            this,
            new(GetUniqueForVisionPerception(sight.EntityId), data.PerceptionEntry));

        SightReleased?.Invoke(this, new() {
            PerceptedSightData = data,
            Sight = sight
        });
    }

    private PerceptionEntry ToPerception(Sight sight)
    {
        var perception = perceptionRecognizer.Recognize(sight);
        perception.Markers.Add(GetUniqueForVisionPerception(sight.EntityId));
        return perception;
    }

    public IEnumerable<PerceptionEntry> GetPerceptions(params string[] markers)
    {
        return perceptionSightDataByEntityId.Values.Select(x => x.PerceptionEntry);
    }
}
