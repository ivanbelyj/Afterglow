using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

using static PerceptionMarkerUtils;

/// <summary>
/// Stores currently percepted vision data
/// </summary>
public class VisionSensoryMemoryStorage : SensoryMemoryStorageBase<Guid, PerceptedSightData, Sight>
{
    public event EventHandler<VisionSensoryMemoryEventArgs> SightCaptured;
    public event EventHandler<VisionSensoryMemoryEventArgs> SightReleased;

    public override string PerceptionSourceKey => CoreSegregatedPerceptionSources.VisionSensoryMemory;

    public VisionSensoryMemoryStorage(IRecognizer<Sight> perceptionRecognizer)
        : base(perceptionRecognizer)
    {
        
    }

    public void CaptureSight(Sight sight, float distance)
    {
        var perceptedSightData = new PerceptedSightData(distance, ToPerception(sight), sight);

        // Debug.Log($"Captured sight: " + sight.VerbalRepresentation + $" [{sight.transform.parent.gameObject.name}]");
        
        Set(sight.EntityId, perceptedSightData);

        Capture(perceptedSightData);

        SightCaptured?.Invoke(this, new() {
            PerceptedSightData = perceptedSightData,
            Sight = sight
        });
    }
    
    public void ReleaseSight(Sight sight)
    {
        // Debug.Log($"Released sight: " + sight.VerbalRepresentation + $" [{sight.transform.parent.gameObject.name}]");
        var data = GetByKey(sight.EntityId);
        RemoveByKey(sight.EntityId);

        Release(data);

        SightReleased?.Invoke(this, new() {
            PerceptedSightData = data,
            Sight = sight
        });
    }

    protected override IEnumerable<string> GetPerceptionMarkers(Sight sight)
        => GetPerceptionIdentifyingMarkers(sight);

    protected override IEnumerable<string> GetPerceptionIdentifyingMarkers(Sight sight)
        => GetVisionIdentifyingMarkers(sight.EntityId);
}
