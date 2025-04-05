using System.Collections.Generic;
using UnityEngine;

public class PerceptionTrackingStorage : MonoBehaviour, IPerceptionTrackingStorage
{
    private readonly List<IPerceptionTrackingHandler> handlers = new();

    public void Track(PerceptionEntry perceptionEntry)
    {
        foreach (var tracker in handlers)
        {
            tracker.Track(perceptionEntry);
        }
    }

    void IPerceptionTrackingStorage.Track(PerceptionEntry perceptionEntry)
    {
        Track(perceptionEntry);
    }

    public void RegisterHandler(IPerceptionTrackingHandler tracker)
    {
        handlers.Add(tracker);
    }
}
