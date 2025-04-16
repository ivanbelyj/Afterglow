using System;
using UnityEngine;

public class PerceptionMarkerUtils
{
    public static string GetEntityUniqueMarker(Guid entityId)
    {
        return $"entity-{entityId}";
    }
    
    public static string[] GetVisionIdentifyingMarkers(Guid entityId)
    {
        return new[] {
            GetEntityUniqueMarker(entityId),
            PerceptionMarkersCore.Vision
        };
    }

    public static string[] GetSensationIdentifyingMarkers(
        Guid sensationId)
    {
        return new[] {
            // Sensations are supposed to be not the most frequent perceptions,
            // so they can be not overwritten
            $"soma-{sensationId}"
        };
    }
}
