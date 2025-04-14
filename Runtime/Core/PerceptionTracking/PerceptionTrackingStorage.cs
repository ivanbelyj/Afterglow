using System.Collections.Generic;
using UnityEngine;

public class PerceptionTrackingStorage : MonoBehaviour, IPerceptionTrackingStorage
{
    private readonly List<IPerceptionTrackingHandler> handlers = new();
    private PerceptionListTracker wholePerceptions;

    public PerceptionListTracker WholePerceptions => wholePerceptions;

    private void Awake()
    {
        // Track all incoming perceptions
        wholePerceptions = new(x => true);
        RegisterHandler(wholePerceptions);
    }

    public void Track(PerceptionEntry perceptionEntry)
    {
        foreach (var tracker in handlers)
        {
            TrackCore(tracker, perceptionEntry);
        }
    }

    void IPerceptionTrackingStorage.Track(PerceptionEntry perceptionEntry)
    {
        Track(perceptionEntry);
    }

    public void RegisterHandler(IPerceptionTrackingHandler tracker)
    {
        handlers.Add(tracker);

        // The handler must receive the relevant perceptions during registration
        foreach (var perception in wholePerceptions.Collection)
        {
            TrackCore(tracker, perception);
        }
    }

    private void TrackCore(
        IPerceptionTrackingHandler tracker,
        PerceptionEntry perceptionEntry)
    {
        tracker.Track(perceptionEntry);
    }
}
