using System;
using UnityEngine;

public static class PerceptionMarkerUtilsCore
{
    public static string GetEntityUniqueMarker(Guid entityId)
    {
        return $"entity-{entityId}";
    }
}
