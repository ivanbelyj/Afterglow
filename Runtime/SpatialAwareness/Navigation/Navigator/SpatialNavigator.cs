using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Provides navigation by receiving spatial awareness from specified providers
/// (like memory, perception, information / data, etc).
/// Does not perform movement or path calculation directly
/// </summary>
public class SpatialNavigator : MonoBehaviour
{
    public event EventHandler<DestinationReachedEventArgs> DestinationReached;
    private List<ISpatialAwarenessPathFinder> spatialAwarenessPathFinders;

    private List<PerceptionBasedNavigationDestination> currentRoute = new();
    private int currentDestinationIndex = -1;

    private void Awake()
    {
        spatialAwarenessPathFinders = GetInitialSpecialAwarenessPathFinders();

        if (spatialAwarenessPathFinders == null || spatialAwarenessPathFinders.Count == 0)
        {
            Debug.LogError($"{nameof(ISpatialAwarenessPathFinder)} is required for {nameof(SpatialNavigator)}");
        }
    }

    public virtual List<ISpatialAwarenessPathFinder> GetInitialSpecialAwarenessPathFinders()
    {
        return GetComponents<ISpatialAwarenessPathFinder>().ToList();
    }

    public void SetTarget(IEnumerable<string> sightMarkers)
    {
        currentRoute = GetBestSpatialAwarenessPathFinder().GetPath(sightMarkers);
        currentDestinationIndex = 0;
    }

    public void AddSpatialAwarenessPathFinder(ISpatialAwarenessPathFinder spatialAwarenessPathFinder)
    {
        spatialAwarenessPathFinders.Add(spatialAwarenessPathFinder);
    }

    public void RemoveSpatialAwarenessPathFinder(ISpatialAwarenessPathFinder spatialAwarenessPathFinder)
    {
        spatialAwarenessPathFinders.Remove(spatialAwarenessPathFinder);
    }

    private ISpatialAwarenessPathFinder GetBestSpatialAwarenessPathFinder()
    {
        return spatialAwarenessPathFinders
            .OrderByDescending(x => x.Priority)
            .First();
    }

    private void Update()
    {
        if (IsCurrentDestinationReached())
        {
            InitiateNextDestination();
        }
    }

    private void InitiateNextDestination()
    {
        var reachedDestination = CurrentDestination;
        MoveNext();
        DestinationReached?.Invoke(this, new() {
            NextDestination = CurrentDestination,
            ReachedDestination = reachedDestination
        });
    }

    private void MoveNext() => currentDestinationIndex++;

    private PerceptionBasedNavigationDestination CurrentDestination
        => currentRoute == null || currentRoute.Count == 0
            || currentDestinationIndex < 0
            || currentDestinationIndex >= currentRoute.Count
        ? null
        : currentRoute[currentDestinationIndex];

    private bool IsCurrentDestinationReached()
    {
        return CurrentDestination == null
            || Vector3.Distance(
                transform.position,
                CurrentDestination.Position.Position)
                < CurrentDestination.Position.Radius;
    }
}
