using UnityEngine;

using static PerceptionEntrySpatialAwarenessDataKeys;

public static class PerceptionEntrySpatialAwarenessExtensions
{
    public static SpatialAwarenessPosition GetSpatialAwarenessPosition(
        this PerceptionEntry perceptionEntry)
    {
        return perceptionEntry.Get<SpatialAwarenessPosition>(Position);
    }
}
