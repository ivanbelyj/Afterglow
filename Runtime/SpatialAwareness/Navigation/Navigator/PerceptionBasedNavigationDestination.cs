using UnityEngine;

public record PerceptionBasedNavigationDestination
{
    public SpatialAwarenessPosition Position { get; set; }
    public PerceptionBasedNavigationType NavigationType { get; set; }
}
