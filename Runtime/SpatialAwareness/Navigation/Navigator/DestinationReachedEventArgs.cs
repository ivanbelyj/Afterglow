using UnityEngine;

public record DestinationReachedEventArgs
{
    public PerceptionBasedNavigationDestination ReachedDestination { get; set; }

    /// <summary>
    /// Null if the final destination is reached
    /// </summary>
    public PerceptionBasedNavigationDestination NextDestination { get; set; }
}
