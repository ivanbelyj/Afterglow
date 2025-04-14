using System;
using UnityEngine;

using static PerceptionMarkerUtilsCore;
using static VisionPerceptionMarkers;

public static class VisionPerceptionMarkerUtils
{
    public static string[] GetIdentifyingMarkersForVisionPerception(Guid entityId)
    {
        return new[] {
            GetEntityUniqueMarker(entityId),
            VisionPerceptionMarkers.Sight
        };
    }
}
