using System;
using System.Collections.Generic;
using UnityEngine;
using static PerceptionEntryCoreDataKeys;

/// <summary>
/// Allows to reduce all construct tracker checks for non-construct perceptions
/// to the one compound check
/// </summary>
public class ConstructPerceptionCompoundTracker : IPerceptionTrackingHandler
{
    private Dictionary<string, ConstructPerceptionTracker> trackersByConstructKey = new();

    public void AddTracker(ConstructPerceptionTracker tracker)
    {
        var constructKey = tracker.ConstructPerception.Get<string>(Construct)
            ?? throw new ArgumentException($"Construct perception must have {Construct} data key / value");
        trackersByConstructKey.Add(constructKey, tracker);
        tracker.ConstructPerception.PerceptionStateChanges += OnPerceptionStateChanged;
    }

    public void Track(PerceptionEntry perception)
    {
        // Probably this condition was added by mistake,
        // but there is a chance that it was added with some reason
        // if (!perception.PerceptionData.ContainsKey(Construct))
        // {
        //     // Skip all trackers because this perception is not a construct
        //     return;
        // }

        foreach (var constructTracker in trackersByConstructKey.Values)
        {
            constructTracker.Track(perception);
        }
    }

    public bool TryGetTracker(string constructKey, out ConstructPerceptionTracker tracker)
    {
        return trackersByConstructKey.TryGetValue(constructKey, out tracker);
    }

    private void Remove(string constructKey)
    {
        trackersByConstructKey.TryGetValue(constructKey, out var tracker);
        tracker.ConstructPerception.PerceptionStateChanges -= OnPerceptionStateChanged;
        trackersByConstructKey.Remove(constructKey);
    }

    protected void OnPerceptionStateChanged(object sender, PerceptionStateChangedEventArgs e)
    {
        if (e.NewState == PerceptionState.Destructed)
        {
            Remove(e.PerceptionEntry.Get<string>(Construct));
        }
    }
}
