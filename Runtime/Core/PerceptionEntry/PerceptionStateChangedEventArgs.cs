using UnityEngine;

public record PerceptionStateChangedEventArgs
{
    public PerceptionEntry PerceptionEntry { get; set; }
    public PerceptionState? OldState { get; set; }
    public PerceptionState NewState { get; set; }

    public PerceptionStateChangedEventArgs(
        PerceptionEntry perceptionEntry,
        PerceptionState? oldState,
        PerceptionState newState)
    {
        PerceptionEntry = perceptionEntry;
        OldState = oldState;
        NewState = newState;
    }
}
