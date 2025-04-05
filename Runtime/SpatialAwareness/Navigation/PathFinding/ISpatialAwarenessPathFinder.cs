using System.Collections.Generic;
using UnityEngine;

public interface ISpatialAwarenessPathFinder
{
    List<PerceptionBasedNavigationDestination> GetPath(
        IEnumerable<string> sightMarkers);
    int Priority { get; }

    void AddProvider(ISpatialAwarenessProvider spatialAwarenessProvider);
    void RemoveProvider(ISpatialAwarenessProvider spatialAwarenessProvider);
}
