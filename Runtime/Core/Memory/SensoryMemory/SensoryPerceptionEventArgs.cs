using UnityEngine;

public record SensoryPerceptionEventArgs
{
    public string UniqueMarker { get; set; }
    public PerceptionEntry PerceptionEntry { get; set; }
    
    public SensoryPerceptionEventArgs(
        string uniqueMarker,
        PerceptionEntry perceptionEntry)
    {
        UniqueMarker = uniqueMarker;
        PerceptionEntry = perceptionEntry;
    }
}
