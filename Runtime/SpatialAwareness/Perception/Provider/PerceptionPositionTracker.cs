using System.Collections.Generic;
using UnityEngine;

internal class PerceptionPositionTracker
{
    private List<(PerceptionEntry, Transform)> trackedPerceptions = new();

    public void Track(PerceptionEntry perceptionEntry, Transform transform)
    {
        trackedPerceptions.Add((perceptionEntry, transform));
    }

    public void Untrack(PerceptionEntry perceptionEntry)
    {
        trackedPerceptions.RemoveAll(x => x.Item1 == perceptionEntry);
    }

    public void Tick()
    {
        foreach (var (perception, transform) in trackedPerceptions)
        {
            var position = perception.GetSpatialAwarenessPosition();
            position.Position = transform.position;
        }
    }
}
