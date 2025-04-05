using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Provides navigation graph, based on knowledge or perception
/// </summary>
public class SpatialAwarenessPathFinder : MonoBehaviour, ISpatialAwarenessPathFinder
{
    [SerializeField]
    private int priority = 1;

    private List<ISpatialAwarenessProvider> spatialAwarenessProviders = new();

    public int Priority => priority;

    public void AddProvider(ISpatialAwarenessProvider spatialAwarenessProvider)
    {
        spatialAwarenessProviders.Add(spatialAwarenessProvider);
    }

    public void RemoveProvider(ISpatialAwarenessProvider spatialAwarenessProvider)
    {
        spatialAwarenessProviders.Remove(spatialAwarenessProvider);
    }

    public List<PerceptionBasedNavigationDestination> GetPath(
        IEnumerable<string> sightMarkers)
    {
        var knownPositions = GetKnownPositions();
        var destinationPosition = GetDestinationByMarkers(knownPositions, sightMarkers);

        if (destinationPosition == null)
        {
            return null;
        }

        return new() { BuildDestination(destinationPosition) };
    }

    private IEnumerable<SpatialAwarenessPosition> GetKnownPositions()
    {
        return spatialAwarenessProviders.SelectMany(
            provider => provider.GetKnownPositions());
    }

    private PerceptionBasedNavigationDestination BuildDestination(
        SpatialAwarenessPosition spatialAwarenessPosition)
    {
        return new PerceptionBasedNavigationDestination() {
            NavigationType = PerceptionBasedNavigationType.Known,
            Position = spatialAwarenessPosition
        };
    }

    private SpatialAwarenessPosition GetDestinationByMarkers(
        IEnumerable<SpatialAwarenessPosition> positions,
        IEnumerable<string> sightMarkers)
    {
        return positions.FirstOrDefault(
            position => sightMarkers.All(x => position.Perception.Markers.Contains(x)));
    }
}
